using MyServer.Util.Concurrent;
using MyProtocol.Dto.Fight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Cache.Fight
{

    public class FightCache
    {
        /// <summary>
        /// user id <-> room id
        /// </summary>
        private Dictionary<int, int> uidRoomIDict = new Dictionary<int, int>();

        /// <summary>
        /// room id <-> room model
        /// </summary>
        private Dictionary<int, FightRoom> idRoomDict = new Dictionary<int, FightRoom>();

        /// <summary>
        /// reused rooms
        /// </summary>
        private Queue<FightRoom> roomQueue = new Queue<FightRoom>();

        /// <summary>
        /// concurrent room id
        /// </summary>
        private ConcurrentInt id = new ConcurrentInt(-1);

        /// <summary>
        /// create room
        /// </summary>
        /// <returns></returns>
        public FightRoom Create(List<int> uidList)
        {
            FightRoom room = null;
            if (roomQueue.Count > 0)
            {
                room = roomQueue.Dequeue();
                room.Init(uidList);
            }
            else
                room = new FightRoom(id.IncreaseGet(), uidList);
            
            foreach (int uid in uidList)
            {
                uidRoomIDict.Add(uid, room.Id);
            }
            idRoomDict.Add(room.Id, room);
            return room;
        }

        /// <summary>
        /// get fight room
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FightRoom GetRoom(int id)
        {
            if (idRoomDict.ContainsKey(id) == false)
            {
                // return null;
                throw new Exception("no this room!");
            }
            FightRoom room = idRoomDict[id];
            return room;
        }

        public bool IsFighting(int userId)
        {
            return uidRoomIDict.ContainsKey(userId);
        }

        /// <summary>
        /// get room by user id
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public FightRoom GetRoomByUId(int uid)
        {
            if (uidRoomIDict.ContainsKey(uid) == false)
            {
                throw new Exception("current user is not in this room!");
            }
            int roomId = uidRoomIDict[uid];
            FightRoom room = GetRoom(roomId);
            return room;
        }

        /// <summary>
        /// destroy room
        /// </summary>
        /// <param name="room"></param>
        public void Destroy(FightRoom room)
        {
            idRoomDict.Remove(room.Id);
            foreach (PlayerDto player in room.PlayerList)
            {
                uidRoomIDict.Remove(player.UserId);
            }
            // init room data
            room.PlayerList.Clear();
            room.LeaveUIdList.Clear();
            room.TableCardList.Clear();
            room.libraryModel.Init();
            room.Multiple = 1;
            room.roundModel.Init();
            // add into reused room list
            roomQueue.Enqueue(room);
        }

    }
}
