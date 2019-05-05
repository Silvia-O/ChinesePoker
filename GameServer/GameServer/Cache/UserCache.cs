using GameServer.Model;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyServer;
using MyServer.Util.Concurrent;

namespace GameServer.Cache
{
    public class UserCache
    {
        /// <summary>
        /// user id <-> user model
        /// </summary>
        private Dictionary<int, UserModel> uidModelDict = new Dictionary<int, UserModel>();

        /// <summary>
        /// acc id <-> user id
        /// </summary>
        private Dictionary<int, int> accIdUIdDict = new Dictionary<int, int>();
        
        
        ConcurrentInt id = new ConcurrentInt(-1);

        /// <summary>
        /// create user
        /// </summary>
        /// <param name="name"></param>
        /// <param name="accountId"></param>
        public void Create(string name, int accountId)
        {
            UserModel model = new UserModel(id.IncreaseGet(), name, accountId);
            
            uidModelDict.Add(model.Id, model);
            accIdUIdDict.Add(model.AccountId, model.Id);
        }

        /// <summary>
        /// whether this account already has a character
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public bool IsExist(int accountId)
        {
            return accIdUIdDict.ContainsKey(accountId);
        }

        /// <summary>
        /// get user model by account id
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public UserModel GetModelByAccountId(int accountId)
        {
            int userId = accIdUIdDict[accountId];
            UserModel model = uidModelDict[userId];
            return model;
        }

        /// <summary>
        /// get user model by client
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public UserModel GetModelByClientPeer(ClientPeer client)
        {
            int id = clientIdDict[client];
            UserModel model = uidModelDict[id];
            return model;
        }

        /// <summary>
        /// get user model by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserModel GetModelById(int userId)
        {
            UserModel user = uidModelDict[userId];
            return user;
        }

        /// <summary>
        /// get client by user id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ClientPeer GetClientPeer(int id)
        {
            return idClientDict[id];
        }

        /// <summary>
        /// get id by client
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public int GetId(ClientPeer client)
        {
            if (!clientIdDict.ContainsKey(client))
            {
                throw new IndexOutOfRangeException("This player is not in the online users dict!");
            }
            return clientIdDict[client];
        }


        /// <summary>
        /// get user id by account id
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public int GetId(int accountId)
        {
            return accIdUIdDict[accountId];
        }

        // online players
        private Dictionary<int, ClientPeer> idClientDict = new Dictionary<int, ClientPeer>();
        private Dictionary<ClientPeer, int> clientIdDict = new Dictionary<ClientPeer, int>();

        /// <summary>
        /// whether user is online by client
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool IsOnline(ClientPeer client)
        {
            return clientIdDict.ContainsKey(client);
        }



        /// <summary>
        /// whether user is online by user id
        /// </summary>
        public bool IsOnline(int id)
        {
            return idClientDict.ContainsKey(id);
        }

        /// <summary>
        /// get online
        /// </summary>
        /// <param name="client"></param>
        /// <param name="id"></param>
        public void Online(ClientPeer client, int id)
        {
            idClientDict.Add(id, client);
            clientIdDict.Add(client, id);
        }

        /// <summary>
        /// update user model
        /// </summary>
        /// <param name="model"></param>
        public void Update(UserModel model)
        {
            uidModelDict[model.Id] = model;
        }

        /// <summary>
        /// get offline
        /// </summary>
        /// <param name="client"></param>
        public void Offline(ClientPeer client)
        {
            int id = clientIdDict[client];
            clientIdDict.Remove(client);
            idClientDict.Remove(id);
        }
    }
}
