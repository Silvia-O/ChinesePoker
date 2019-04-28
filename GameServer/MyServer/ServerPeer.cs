using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyServer
{
    /// <summary>
    /// Server 
    /// </summary>
    public class ServerPeer
    {
        private Socket serverSocket;
        private Semaphore acceptSemaphore;
        private ClientPeerPool clientPeerPool;
        private IApplication app;

        public void SetApplication(IApplication app)
        {
            this.app = app;
        }


        #region connect

        /// <summary>
        /// Start Server
        /// </summary>
        /// <param name="port"></param>
        /// <param name="matCount"></param>
        public void Start(int port, int maxCount)
        {
            try
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                acceptSemaphore = new Semaphore(maxCount, maxCount);

                // new all the connecting objects
                clientPeerPool = new ClientPeerPool(maxCount);
                ClientPeer tmpClientPeer = null;
                for (int i = 0; i < maxCount; i++)
                {
                    tmpClientPeer = new ClientPeer();
                    tmpClientPeer.ReceiveArgs.Completed += Receive_Completed;
                    tmpClientPeer.receiveCompleted = ReceiveCompleted;
                    tmpClientPeer.sendDisconnect = Disconnect;
                    clientPeerPool.Enqueue(tmpClientPeer);
                }

                serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
                serverSocket.Listen(10);

                Console.WriteLine("Server starts ...");

                StartAccept(null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// accept connection between client
        /// </summary>
        /// <param name="e"></param>
        private void StartAccept(SocketAsyncEventArgs e)
        {
            if (e == null)
            {
                e = new SocketAsyncEventArgs();
                e.Completed += AcceptCompleted;
            }

            bool isExecuting = serverSocket.AcceptAsync(e);
            // true: async event is executing
            // false: async event is done
            if(isExecuting == false)
            {
                HandleAccept(e);
            }

        }
        /// <summary>
        /// process accepting connection from client
        /// </summary>
        /// <param name="e"></param>
        private void HandleAccept(SocketAsyncEventArgs e)
        {
            // restrict thread access
            acceptSemaphore.WaitOne();

            // get client object
            // Socket clientSocket = e.AcceptSocket;
            ClientPeer client = clientPeerPool.Dequeue();
            client.ClientSocket = e.AcceptSocket;

            // tell application
            // app.OnConnect(client);

            // save and process
            StartReceive(client);

            // recursive call
            e.AcceptSocket = null;
            StartAccept(e);

        }

        /// <summary>
        /// be triggered once connection accepting async evnet finishes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AcceptCompleted(object sender, SocketAsyncEventArgs e)
        {
            HandleAccept(e);
        }
        #endregion

        #region send data
        private void StartReceive(ClientPeer client)
        {
            try
            {
                bool isExecuting = client.ClientSocket.ReceiveAsync(client.ReceiveArgs);
                if(isExecuting == false)
                {
                    HandleReceive(client.ReceiveArgs);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void HandleReceive(SocketAsyncEventArgs e)
        {
            ClientPeer client = e.UserToken as ClientPeer;
            // decide whether packet is received sucessfully 
            if(client.ReceiveArgs.SocketError == SocketError.Success && client.ReceiveArgs.BytesTransferred > 0)
            {
                // clone into array
                byte[] byteArray = new byte[client.ReceiveArgs.BytesTransferred];
                Buffer.BlockCopy(client.ReceiveArgs.Buffer, 0, byteArray, 0, client.ReceiveArgs.BytesTransferred);
                client.StartReceive(byteArray);
                StartReceive(client);
            }
            // disconnected 
            else if(client.ReceiveArgs.BytesTransferred == 0)
            {
                
                if(client.ReceiveArgs.SocketError == SocketError.Success)
                {
                    // client initiative to disconnect
                    Disconnect(client, "client initiative to disconnect");
                }
                else
                {
                    // connection broken passively
                    Disconnect(client, client.ReceiveArgs.SocketError.ToString());
                }
            }
            

        }
        /// <summary>
        /// be triggered once data receiving async evnet finishes
        /// </summary>
        /// <param name="e"></param>
        private void Receive_Completed(object sender, SocketAsyncEventArgs e)
        {
            HandleReceive(e);
        }

        /// <summary>
        /// one packet completed resolving
        /// </summary>
        /// <param name="client"></param>
        /// <param name="value">resolved packet</param>
        private void ReceiveCompleted(ClientPeer client, SocketMsg msg)
        {
            // for application using
            app.OnReceive(client, msg);
        }
        #endregion

        #region receive data

        #endregion

        #region disconnect

        /// <summary>
        /// disconnect with client
        /// </summary>
        /// <param name="cleint"></param>
        /// <param name="reason">reason for disconneting</param>
        public void Disconnect(ClientPeer client, string reason)
        {
            try
            {
                // clear 
                if (client == null)
                    throw new Exception("Current client object is null. Disconnecting is failed.");

                // tell application
                app.OnDisconnect(client);

                client.Disconnect();
                // retrieve client object
                clientPeerPool.Enqueue(client);
                acceptSemaphore.Release();

            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        #endregion


    }
}
