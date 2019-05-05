using MyServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Cache.Match
{
    public class MatchRoom
    {
        
        public int Id { get; private set; }

        
        public Dictionary<int, ClientPeer> UIdClientDict { get; private set; }

        /// <summary>
        /// players already ready
        /// </summary>
        public List<int> ReadyUIdList { get; private set; }

        public MatchRoom(int id)
        {
            this.Id = id;
            this.UIdClientDict = new Dictionary<int, ClientPeer>();
            this.ReadyUIdList = new List<int>();
        }

        /// <summary>
        /// whether room is full
        /// </summary>
        /// <returns></returns>
        public bool IsFull()
        {
            return UIdClientDict.Count == 3;
        }

        /// <summary>
        /// whether room is empty
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return UIdClientDict.Count == 0;
        }

        /// <summary>
        /// whether all players are ready
        /// </summary>
        /// <returns></returns>
        public bool IsAllReady()
        {
            return ReadyUIdList.Count == 3;
        }

        public List<int> GetUIdList()
        {
            return UIdClientDict.Keys.ToList();
        }

        /// <summary>
        /// enter room
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="client"></param>
        public void Enter(int userId, ClientPeer client)
        {
            UIdClientDict.Add(userId, client);
        }

        /// <summary>
        /// leave room
        /// </summary>
        /// <param name="userId"></param>
        public void Leave(int userId)
        {
            UIdClientDict.Remove(userId);
        }

        /// <summary>
        /// get ready
        /// </summary>
        /// <param name="userId"></param>
        public void Ready(int userId)
        {
            ReadyUIdList.Add(userId);
        }

        /// <summary>
        /// breadcast player info
        /// </summary>
        /// <param name="opCode"></param>
        /// <param name="subCode"></param>
        /// <param name="value"></param>
        /// <param name="exClient"></param>
        public void Broadcast(int opCode, int subCode, object value, ClientPeer exClient = null)
        {
            SocketMsg msg = new SocketMsg(opCode, subCode, value);
            byte[] data = EncodeTool.EncodeMsg(msg);
            byte[] packet = EncodeTool.EncodePacket(data);

            foreach (var client in UIdClientDict.Values)
            {
                if (client == exClient)
                    continue;

                client.Send(packet);
            }
        }

    }
}
