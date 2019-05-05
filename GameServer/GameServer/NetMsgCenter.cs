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
        AccountHandler account = new AccountHandler();
        UserHandler user = new UserHandler();
        MatchHandler match = new MatchHandler();
        FightHandler fight = new FightHandler();

        public NetMsgCenter()
        {
            match.startFight += fight.StartFight;
        }


        public void OnDisconnect(ClientPeer client)
        {
            account.OnDisconnect(client);
            user.OnDisconnect(client);
            match.OnDisconnect(client);
            fight.OnDisconnect(client);

        }
        
        public void OnReceive(ClientPeer client, SocketMsg msg)
        {

            switch (msg.OpCode)
            {
                case OpCode.ACCOUNT:
                    account.OnReceive(client, msg.SubCode, msg.Value);
                    break;
                case OpCode.USER:
                    user.OnReceive(client, msg.SubCode, msg.Value);
                    break;
                case OpCode.MATCH:
                    match.OnReceive(client, msg.SubCode, msg.Value);
                    break;
                case OpCode.FIGHT:
                    fight.OnReceive(client, msg.SubCode, msg.Value);
                    break;
                default:
                    break;
            }

        }
    }
}
