using GameServer.Cache;
using MyProtocol.Code;
using MyProtocol.Dto;
using MyServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Logic
{
    public class AccountHandler: IHandler
    {
        AccountCache AccCache = Caches.Account;

        public void OnReceive(ClientPeer client, int subCode, object value)
        {
            switch (subCode)
            {
                case AccountCode.REGISTER_CREQ:
                    AccountDto rdto = value as AccountDto;
                    Register(client, rdto.Acc, rdto.Pwd);
                    break;
                case AccountCode.LOGIN_CREQ:
                    AccountDto ldto = value as AccountDto;
                    Login(client, ldto.Acc, ldto.Pwd);
                    break;
                default:
                    break;
            }

        }

        /// <summary>
        /// register account
        /// </summary>
        /// <param name="client"></param>
        /// <param name="acc"></param>
        /// <param name="pwd"></param>
        public void Register(ClientPeer client, string acc, string pwd)
        {
            SingleThread.Instance.SingleExecute(() =>
            {   
                // account is already existed
                if (AccCache.IsExist(acc))
                {
                    client.Send(OpCode.ACCOUNT, AccountCode.REGISTER_SRES, -1);
                    return;
                }
                // account is not legal
                if (string.IsNullOrEmpty(acc))
                {
                    client.Send(OpCode.ACCOUNT, AccountCode.REGISTER_SRES, -2);
                    return;
                }
                // password is not legal
                if (string.IsNullOrEmpty(pwd) || pwd.Length < 6 || pwd.Length > 16)
                {
                    client.Send(OpCode.ACCOUNT, AccountCode.REGISTER_SRES, -3);
                    return;
                }
                // able to register
                AccCache.Create(acc, pwd);
                client.Send(OpCode.ACCOUNT, AccountCode.REGISTER_SRES, 0);
            });
            
        }

        /// <summary>
        /// login account
        /// </summary>
        /// <param name="client"></param>
        /// <param name="acc"></param>
        /// <param name="pwd"></param>
        public void Login(ClientPeer client, string acc, string pwd)
        {
            SingleThread.Instance.SingleExecute(() =>
            {
                // account is not exsited
                if (!AccCache.IsExist(acc))
                {
                    client.Send(OpCode.ACCOUNT, AccountCode.LOGIN_SRES, -1);
                    return;
                }
                // account is already online
                if (AccCache.IsOnline(acc))
                {
                    client.Send(OpCode.ACCOUNT, AccountCode.LOGIN_SRES, -2);
                    return;
                }
                // account and password do not match
                if (!AccCache.IsMatch(acc, pwd))
                {
                    client.Send(OpCode.ACCOUNT, AccountCode.LOGIN_SRES, -3);
                    return;
                }
                // able to login
                AccCache.Online(client, acc);
                client.Send(OpCode.ACCOUNT, AccountCode.LOGIN_SRES, 0);
            });

        }



        public void OnDisconnect(ClientPeer client)
        {
            if(AccCache.IsOnline(client))
                AccCache.Offline(client);
        }
    }
}
