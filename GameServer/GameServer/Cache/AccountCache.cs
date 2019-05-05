using GameServer.Model;
using MyServer;
using MyServer.Util.Concurrent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Cache
{
    public class AccountCache
    {
        #region acc and pwd

        private Dictionary<string, AccountModel> AccModelDict = new Dictionary<string, AccountModel>();

        public bool IsExist(string acc)
        {
            return AccModelDict.ContainsKey(acc);
        }

        private ConcurrentInt id = new ConcurrentInt(-1);

        /// <summary>
        /// create new account
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="pwd"></param>
        public void Create(string acc, string pwd)
        {
            AccountModel model = new AccountModel(id.IncreaseGet(), acc, pwd);
            AccModelDict.Add(model.Acc, model);
        }

        /// <summary>
        /// get corresponding model of account
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public AccountModel GetAccModel(string acc)
        {
            return AccModelDict[acc];
        }


        public bool IsMatch(string acc, string pwd)
        {
            AccountModel model = GetAccModel(acc);
            return model.Pwd == pwd;
        }

        #endregion

        #region online and offline

        private Dictionary<string, ClientPeer> AccClientDict = new Dictionary<string, ClientPeer>();
        private Dictionary<ClientPeer, string> ClientAccDict = new Dictionary<ClientPeer, string>();

        public bool IsOnline(string acc)
        {
            return AccClientDict.ContainsKey(acc);
        }

        public bool IsOnline(ClientPeer client)
        {
            return ClientAccDict.ContainsKey(client);
        }

        public void Online(ClientPeer client, string acc)
        {
            AccClientDict.Add(acc, client);
            ClientAccDict.Add(client, acc);
        }

        public void Offline(ClientPeer client)
        {
            string acc = ClientAccDict[client];
            AccClientDict.Remove(acc);
            ClientAccDict.Remove(client);
        }

        public void Offline(string acc)
        {
            ClientPeer client = AccClientDict[acc];
            AccClientDict.Remove(acc);
            ClientAccDict.Remove(client);
        }

        #endregion

        public int GetAccId(ClientPeer client)
        {
            string acc = ClientAccDict[client];
            AccountModel model = AccModelDict[acc];
            return model.Id;
        }
    }
}
