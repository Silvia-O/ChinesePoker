using GameServer.Logic;
using MyServer;
using MyProtocol.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    public class NetMsgCenter : IApplication
    {
        IHandler account = new AccountHandler();

        public void OnDisconnect(ClientPeer client)
        {
            account.OnDisconnect(client);
        }
        
        public void OnReceive(ClientPeer client, SocketMsg msg)
        {

            switch (msg.OpCode)
            {
                case OpCode.ACCOUNT:
                    account.OnReceive(client, msg.SubCode, msg.Value);
                    break;
                default:
                    break;
            }

        }
    }
}
