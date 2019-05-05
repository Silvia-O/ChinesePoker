using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;

public class ClientPeer
{
    private Socket socket;
    private string ip;
    private int port;

    /// <summary>
    /// construct client socket object
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    public ClientPeer(string ip, int port)
    {
        try
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.ip = ip;
            this.port = port;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    public void Connect()
    {
        try
        {
            socket.Connect(ip, port);
            Debug.Log("Succeed to connect server!");

            // async receive data
            StartReceive();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    private byte[] receiveBuffer = new byte[1024];

    private List<byte> dataCache = new List<byte>();

    private bool isHandleReceive = false;

    public Queue<SocketMsg> SocketMsgQueue = new Queue<SocketMsg>();

    #region receive data

    private void StartReceive()
    {
        if (socket == null && socket.Connected == false)
        {
            Debug.LogError("Fail to connect!");
            return;
        }

        socket.BeginReceive(receiveBuffer, 0, 1024, SocketFlags.None, ReceiveCallBack, socket);
    }

    /// <summary>
    /// call back once received msg
    /// </summary>
    /// <param name="ar"></param>
    private void ReceiveCallBack(IAsyncResult ar)
    {
        try
        {
            int length = socket.EndReceive(ar);
            byte[] byteArray = new byte[length];
            Buffer.BlockCopy(receiveBuffer, 0, byteArray, 0, length);
            dataCache.AddRange(byteArray);
            // process received data
            if (isHandleReceive == false)
                HandleReceive();
            
            StartReceive();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    /// <summary>
    /// process received data
    /// </summary>
    private void HandleReceive()
    {
        isHandleReceive = true;
        byte[] data = EncodeTool.DecodePacket(ref dataCache);

        if (data == null)
        {
            isHandleReceive = false;
            return;
        }

        SocketMsg msg = EncodeTool.DecodeMsg(data);
        SocketMsgQueue.Enqueue(msg);

        HandleReceive();
    }

    #endregion

    #region send data

    public void Send(int opCode, int subCode, object value)
    {
        SocketMsg msg = new SocketMsg(opCode, subCode, value);
        
        Send(msg);
    }

    public void Send(SocketMsg msg)
    {
        byte[] data = EncodeTool.EncodeMsg(msg);
        byte[] packet = EncodeTool.EncodePacket(data);

        try
        {
            socket.Send(packet);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    #endregion
}