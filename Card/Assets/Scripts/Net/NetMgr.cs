using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using MyProtocol.Code;
using UnityEditor.U2D;
using UnityEngine;

public class NetMgr : ManagerBase
{
    public static NetMgr Instance = null;

    private ClientPeer client = new ClientPeer("127.0.0.1", 6666);
    
    #region handle events msg from server
    private void Start()
    {
        client.Connect();
    }

    private void Update()
    {
        if (client == null)
        {
            return;
        }

        while (client.SocketMsgQueue.Count > 0)
        {
            SocketMsg msg = client.SocketMsgQueue.Dequeue();
            HandleSocketMsg(msg);
        }
    }
    
    HandlerBase accountHandler = new AccountHandler();
    HandlerBase userHandler = new UserHandler();
    HandlerBase matchHandler = new MatchHandler();
    HandlerBase fightHandler = new FightHandler();
    
    private void HandleSocketMsg(SocketMsg msg)
    {
        switch (msg.OpCode)
                {
                    case OpCode.ACCOUNT:
                        accountHandler.OnReceive(msg.SubCode, msg.Value);
                        break;
                    case OpCode.USER:
                        userHandler.OnReceive(msg.SubCode, msg.Value);
                        break;
                    case OpCode.MATCH:
                        matchHandler.OnReceive(msg.SubCode, msg.Value);
                        break;
                    case OpCode.FIGHT:
                        fightHandler.OnReceive(msg.SubCode, msg.Value);
                        break;
                    default:
                        break;
                }
    }
    #endregion
    
    #region handle events msg from client to server
    private void Awake()
    {
        Instance = this;
        
        Add(0, this);
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case 0:
                client.Send(message as SocketMsg);
                break;
            default:
                break;
        }
    }
    #endregion
    
    
}