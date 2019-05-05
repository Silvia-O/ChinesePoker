using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MyServer
{
    public class ClientPeer
    {
        public Socket ClientSocket { get; set; }

        public ClientPeer()
        {
            this.ReceiveArgs = new SocketAsyncEventArgs();
            this.ReceiveArgs.UserToken = this;
            this.ReceiveArgs.SetBuffer(new byte[1024], 0, 1024);
            this.SendArgs = new SocketAsyncEventArgs();
            this.SendArgs.Completed += Send_Completed;
        }

        #region receive data

        /// <summary>
        /// call back once message resolved 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="msg"></param>
        public delegate void ReceiveCompleted(ClientPeer client, SocketMsg msg);
        public ReceiveCompleted receiveCompleted;

        /// <summary>
        /// save data into cache
        /// </summary>
        private List<byte> dataCache = new List<byte>();

        /// <summary>
        /// received async socket request
        /// </summary>
        public SocketAsyncEventArgs ReceiveArgs { get; set; }


        /// <summary>
        /// whether is handling receiving packet 
        /// </summary>
        private bool isHandleReceive = false;


        public void StartReceive(byte[] packet)
        {
            dataCache.AddRange(packet);
            if (!isHandleReceive)
                HandleReceive();
        }

        private void HandleReceive()
        {
            isHandleReceive = true;
            byte[] data = EncodeTool.DecodePacket(ref dataCache);

            if(data == null)
            {
                isHandleReceive = false;
                return;
            }

            SocketMsg msg = EncodeTool.DecodeMsg(data);
            // callback
            if(receiveCompleted != null)
            {
                receiveCompleted(this, msg);
            }

            HandleReceive();
        }
        // packet sticking & spliting
        // to tackle: define packet head and tail 
        // msg head: msg length
        // msg tail: msg content
        #endregion

        #region send data
        
        /// <summary>
        /// call back once disconnecting when sending data 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="reason"></param>
        public delegate void SendDisconnect(ClientPeer client, string reason);
        public SendDisconnect sendDisconnect;

        /// <summary>
        /// sent async socket request
        /// </summary>
        public SocketAsyncEventArgs SendArgs;

        /// <summary>
        /// whether is handling sending packet
        /// </summary>
        private bool isHandleSend = false;

        private Queue<byte[]> sendQueue = new Queue<byte[]>();

        public void Send(int opCode, int subCode, object value)
        {
            SocketMsg msg = new SocketMsg(opCode, subCode, value);
            byte[] data = EncodeTool.EncodeMsg(msg);
            byte[] packet = EncodeTool.EncodePacket(data);

            Send(packet);
        }

        public void Send(byte[] packet)
        {
            sendQueue.Enqueue(packet);
            if (!isHandleSend)
                Send();
        }

        private void Send()
        {
            isHandleSend = true;
            
            // stop sending when no data to  be sent
            if(sendQueue.Count == 0)
            {
                isHandleSend = false;
                return;
            }

            byte[] packet = sendQueue.Dequeue();

            SendArgs.SetBuffer(packet, 0, packet.Length);
            bool isExecuting = ClientSocket.SendAsync(SendArgs);
            if (isExecuting == false)
            {
                HandleSend();
            }
        }
          
        private void Send_Completed(object sender, SocketAsyncEventArgs e)
        {
            HandleSend();
        }

        /// <summary>
        /// call once async sending request finishes 
        /// </summary>
        private void HandleSend()
        {
            if(SendArgs.SocketError != SocketError.Success)
            {
                // client disconnect
                sendDisconnect(this, SendArgs.SocketError.ToString());
            }
            else
            {
                Send();
            }
        }
        #endregion

        #region disconnect

        public void Disconnect()
        {
            // clear data cache
            dataCache.Clear();
            isHandleReceive = false;
            sendQueue.Clear();
            isHandleSend = false;

            ClientSocket.Shutdown(SocketShutdown.Both);
            ClientSocket.Close();
            ClientSocket = null;
        }

        #endregion
    }
}
