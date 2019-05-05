using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyServer;
using MyProtocol.Code;
using GameServer.Cache;
using MyProtocol.Dto;
using GameServer.Model;
using MyProtocol.Dto;
using GameServer.Cache.Match;

namespace GameServer.Logic
{
    public delegate void StartFight(List<int> uidList);

    public class MatchHandler : IHandler
    {
        public StartFight startFight;

        private MatchCache matchCache = Caches.Match;
        private UserCache userCache = Caches.User;

        public void OnDisconnect(ClientPeer client)
        {
            if (!userCache.IsOnline(client))
            {
                return;
            }
            int userId = userCache.GetId(client);
            if (matchCache.IsMatching(userId))
            {
                Leave(client);
            }
        }

        public void OnReceive(ClientPeer client, int subCode, object value)
        {
            switch (subCode)
            {
                case MatchCode.ENTER_CREQ:
                    //enter(client, (int)value);
                    Enter(client);
                    break;
                case MatchCode.LEAVE_CREQ:
                    Leave(client);
                    break;
                case MatchCode.READY_CREQ:
                    Ready(client);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// enter room
        /// </summary>
        /// <param name="client"></param>
        private void Enter(ClientPeer client)
        {
            SingleThread.Instance.SingleExecute(
                delegate ()
                {
                    if (!userCache.IsOnline(client))
                        return;
                    int userId = userCache.GetId(client);
                    
                    if (matchCache.IsMatching(userId))
                    {
                        client.Send(OpCode.MATCH, MatchCode.ENTER_SRES, -1);
                        return;
                    }
                    // able to enter room
                    MatchRoom room = matchCache.Enter(userId, client);

                    // broadcast entering player info to other players
                    #region construct a UserDto
                    UserModel model = userCache.GetModelById(userId);
                    UserDto userDto = new UserDto(model.Id, model.Name, model.Bean, model.WinCount, model.LoseCount, model.RunCount, model.Lv, model.Exp);
                    #endregion
                    room.Broadcast(OpCode.MATCH, MatchCode.ENTER_BRO, userDto, client);
                    
                    MatchRoomDto dto = MakeRoomDto(room);
                    client.Send(OpCode.MATCH, MatchCode.ENTER_SRES, dto);

                    Console.WriteLine("有玩家进入匹配房间.");
                }
                );
        }


        /// <summary>
        /// leave room
        /// </summary>
        /// <param name="client"></param>
        private void Leave(ClientPeer client)
        {
            SingleThread.Instance.SingleExecute(
                delegate ()
                {
                    if (!userCache.IsOnline(client))
                        return;
                    int userId = userCache.GetId(client);
                    
                    if (matchCache.IsMatching(userId) == false)
                    {
                        return;
                    }

                    MatchRoom room = matchCache.Leave(userId);
                    // broadcast left player info to other players
                    room.Broadcast(OpCode.MATCH, MatchCode.LEAVE_BRO, userId);

                    Console.WriteLine("有玩家离开匹配房间.");
                });
        }
    

        /// <summary>
        /// get ready
        /// </summary>
        /// <param name="client"></param>
        private void Ready(ClientPeer client)
        {
            SingleThread.Instance.SingleExecute(
                () =>
                {
                    if (userCache.IsOnline(client) == false)
                        return;

                    int userId = userCache.GetId(client);

                    if (matchCache.IsMatching(userId) == false)
                        return;

                    MatchRoom room = matchCache.GetRoom(userId);
                    room.Ready(userId);
                    //  broadcast ready player info to other players
                    room.Broadcast(OpCode.MATCH, MatchCode.READY_BRO, userId);

                    
                    if (room.IsAllReady())
                    {
                        // start fighting 
                        startFight(room.GetUIdList());
                        // broadcast start fighting info to all players
                        room.Broadcast(OpCode.MATCH, MatchCode.START_BRO, null);
                        // destroy matching room
                        matchCache.Destroy(room);
                    }
                }
                );
        }



        private MatchRoomDto MakeRoomDto(MatchRoom room)
        {
            MatchRoomDto dto = new MatchRoomDto();
            
            foreach (var uid in room.UIdClientDict.Keys)
            {
                UserModel model = userCache.GetModelById(uid);
                UserDto userDto = new UserDto(model.Id, model.Name, model.Bean, model.WinCount, model.LoseCount, model.RunCount, model.Lv, model.Exp);
                dto.UIdUserDict.Add(uid, userDto);
                dto.UIdList.Add(uid);
            }
            dto.ReadyUIdList = room.ReadyUIdList;
            return dto;
        }


    }
}
