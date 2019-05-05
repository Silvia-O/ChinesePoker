using MyServer;
using MyServer.Util.Concurrent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Cache.Match
{
    public class MatchCache
    {
        /// <summary>
        /// user id <-> room id
        /// </summary>
        private Dictionary<int, int> uidRoomIdDict = new Dictionary<int, int>();

        /// <summary>
        /// room id <-> room model
        /// </summary>
        private Dictionary<int, MatchRoom> roomIdModelDict = new Dictionary<int, MatchRoom>();

        /// <summary>
        /// reused rooms
        /// </summary>
        private Queue<MatchRoom> roomQueue = new Queue<MatchRoom>();

        /// <summary>
        /// concurrent room id
        /// </summary>
        private ConcurrentInt id = new ConcurrentInt(-1);

        /// <summary>
        /// enter room
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public MatchRoom Enter(int userId, ClientPeer client)
        {
            // waiting room
            foreach (MatchRoom mr in roomIdModelDict.Values)
            {
                if (mr.IsFull())
                    continue;
                // room if not full
                mr.Enter(userId, client);
                uidRoomIdDict.Add(userId, mr.Id);
                return mr;
            }
            // no waiting room
            // create a new room 
            MatchRoom room = null;
            
            if (roomQueue.Count > 0)
                room = roomQueue.Dequeue();
            else
                room = new MatchRoom(id.IncreaseGet());

            room.Enter(userId, client);
            roomIdModelDict.Add(room.Id, room);
            uidRoomIdDict.Add(userId, room.Id);
            return room;
        }

        /// <summary>
        /// leave room
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public MatchRoom Leave(int userId)
        {
            int roomId = uidRoomIdDict[userId];
            MatchRoom room = roomIdModelDict[roomId];
            room.Leave(userId);
            
            uidRoomIdDict.Remove(userId);
            if (room.IsEmpty())
            {
                // add into reused rooms
                roomIdModelDict.Remove(roomId);
                roomQueue.Enqueue(room);
            }
            return room;
        }

        /// <summary>
        /// whether user is in matching room
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsMatching(int userId)
        {
            return uidRoomIdDict.ContainsKey(userId);
        }

        /// <summary>
        /// get the room player in
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public MatchRoom GetRoom(int userId)
        {
            int roomId = uidRoomIdDict[userId];
            MatchRoom room = roomIdModelDict[roomId];
            return room;
        }

        /// <summary>
        /// destroy room
        /// </summary>
        public void Destroy(MatchRoom room)
        {
            roomIdModelDict.Remove(room.Id);
            foreach (var userId in room.UIdClientDict.Keys)
            {
                uidRoomIdDict.Remove(userId);
            }
            // clear
            room.UIdClientDict.Clear();
            room.ReadyUIdList.Clear();
            roomQueue.Enqueue(room);
        }
    }
}
