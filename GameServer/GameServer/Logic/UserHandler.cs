using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyServer;
using MyProtocol.Code;
using GameServer.Cache;
using GameServer.Model;
using MyProtocol.Dto;

namespace GameServer.Logic
{
    public class UserHandler : IHandler
    {
        private UserCache userCache = Caches.User;
        private AccountCache accountCache = Caches.Account;

        public void OnDisconnect(ClientPeer client)
        {
            if (userCache.IsOnline(client))
                userCache.Offline(client);
        }

        public void OnReceive(ClientPeer client, int subCode, object value)
        {
            switch (subCode)
            {
                case UserCode.CREATE_CREQ:
                    Create(client, value.ToString());
                    break;
                case UserCode.GET_INFO_CREQ:
                    GetInfo(client);
                    break;
                case UserCode.ONLINE_CREQ:
                    Online(client);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// create character
        /// </summary>
        /// <param name="client"></param>
        /// <param name="name"></param>
        private void Create(ClientPeer client, string name)
        {
            SingleThread.Instance.SingleExecute(
                delegate ()
                {
                    // not online
                    if (!accountCache.IsOnline(client))
                    {
                        client.Send(OpCode.USER, UserCode.CREATE_SRES, -1);
                        return;
                    }
                    
                    int accountId = accountCache.GetAccId(client);
                    
                    // whether this account already has character
                    if (userCache.IsExist(accountId))
                    {
                        client.Send(OpCode.USER, UserCode.CREATE_SRES, -2);
                        return;
                    }
                    // able to create character
                    userCache.Create(name, accountId);
                    client.Send(OpCode.USER, UserCode.CREATE_SRES, 0);
                }
           );
        }

        /// <summary>
        /// get character info
        /// </summary>
        /// <param name="client"></param>
        private void GetInfo(ClientPeer client)
        {
            SingleThread.Instance.SingleExecute(
                  delegate ()
                  {
                      // not online
                      if (!accountCache.IsOnline(client))
                      {
                          //client.Send(OpCode.USER, UserCode.GET_INFO_SRES, null);
                          return;
                      }

                      int accountId = accountCache.GetAccId(client);

                      // this account has not created character
                      if (userCache.IsExist(accountId) == false)
                      {
                          client.Send(OpCode.USER, UserCode.GET_INFO_SRES, null);
                          return;
                      }

                      // online character
                      if (userCache.IsOnline(client) == false)
                      {
                          Online(client);
                      }
                      // able to get info
                      UserModel model = userCache.GetModelByAccountId(accountId);
                      UserDto dto = new UserDto(model.Id, model.Name, model.Bean, model.WinCount, model.LoseCount, model.RunCount, model.Lv, model.Exp);
                      client.Send(OpCode.USER, UserCode.GET_INFO_SRES, dto);
                  }
             );

        }

        /// <summary>
        /// online character 
        /// </summary>
        /// <param name="client"></param>
        private void Online(ClientPeer client)
        {
            SingleThread.Instance.SingleExecute(
                   delegate ()
                   {
                       // not online
                       if (!accountCache.IsOnline(client))
                       {
                           client.Send(OpCode.USER, UserCode.ONLINE_SRES, -1);
                           return;
                       }

                       int accountId = accountCache.GetAccId(client);

                       // this account has not created character
                       if (userCache.IsExist(accountId) == false)
                       {
                           client.Send(OpCode.USER, UserCode.ONLINE_SRES, -2);
                           return;
                       }
                       // able to online
                       int userId = userCache.GetId(accountId);
                       userCache.Online(client, userId);
                       client.Send(OpCode.USER, UserCode.ONLINE_SRES, 0);
                   }
              );
        }

    }
}
