using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyServer;
using GameServer.Cache.Fight;
using GameServer.Cache;
using MyProtocol.Code;
using MyProtocol.Dto.Fight;
using GameServer.Model;

namespace GameServer.Logic
{
    public class FightHandler : IHandler
    {
        public FightCache fightCache = Caches.Fight;
        public UserCache userCache = Caches.User;

        public void OnDisconnect(ClientPeer client)
        {
            Leave(client);
        }

        public void OnReceive(ClientPeer client, int subCode, object value)
        {
            switch (subCode)
            {
                case FightCode.GRAB_LANDLORD_CREQ:
                    bool result = (bool)value;
                    GrabLandlord(client, result);
                    break;
                case FightCode.DEAL_CREQ:
                    Deal(client, value as DealDto);
                    break;
                case FightCode.PASS_CREQ:
                    Pass(client);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// user leave
        /// </summary>
        /// <param name="client"></param>
        private void Leave(ClientPeer client)
        {
            SingleThread.Instance.SingleExecute(
                () =>
                {
                    if (userCache.IsOnline(client) == false)
                        return;
                    // ensure player is online fighting
                    int userId = userCache.GetId(client);
                    if (fightCache.IsFighting(userId) == false)
                    {
                        return;
                    }
                    FightRoom room = fightCache.GetRoomByUId(userId);

                    room.LeaveUIdList.Add(userId);
                    Broadcast(room, OpCode.FIGHT, FightCode.LEAVE_BRO, userId);

                    if (room.LeaveUIdList.Count == 3)
                    {
                        // add runcount
                        for (int i = 0; i < room.LeaveUIdList.Count; i++)
                        {
                            UserModel um = userCache.GetModelById(room.LeaveUIdList[i]);
                            um.RunCount++;
                            um.Bean -= (room.Multiple * 1000) * 3;
                            um.Bean += 0;
                            userCache.Update(um);
                        }

                        // destroy room data in cache layer
                        fightCache.Destroy(room);
                    }
                });
        }
        
        private void Pass(ClientPeer client)
        {
            SingleThread.Instance.SingleExecute(
                () =>
                {
                    if (userCache.IsOnline(client) == false)
                        return;
                    // ensure player is online fighting 
                    int userId = userCache.GetId(client);
                    
                    FightRoom room = fightCache.GetRoomByUId(userId);
                    
                    if (room.roundModel.BiggestUId == userId)
                    {
                        // no way but to pass
                        client.Send(OpCode.FIGHT, FightCode.PASS_SRES, -1);
                        return;
                    }
                    else
                    {
                        // choose to pass
                        client.Send(OpCode.FIGHT, FightCode.PASS_SRES, 0);
                        Turn(room);
                    }
                });
        }

        
        private void Deal(ClientPeer client, DealDto dto)
        {
            SingleThread.Instance.SingleExecute(
                delegate ()
                {
                    if (userCache.IsOnline(client) == false)
                        return;
                    // ensure player is online fighting
                    int userId = userCache.GetId(client);
                    if (userId != dto.UserId)
                    {
                        return;
                    }
                    FightRoom room = fightCache.GetRoomByUId(userId);

                    
                    if (room.LeaveUIdList.Contains(userId))
                    {
                        Turn(room);
                    }
                    bool canDeal = room.DealCard(dto.Type, dto.Weight, dto.Length, userId, dto.SelectCardList);
                    if (canDeal == false)
                    {
                        client.Send(OpCode.FIGHT, FightCode.DEAL_SRES, -1);
                        return;
                    }
                    else
                    {
                        client.Send(OpCode.FIGHT, FightCode.DEAL_SRES, 0);
                 
                        List<CardDto> remainCardList = room.GetPlayerModel(userId).CardList;
                        dto.RemainCardList = remainCardList;
                        Broadcast(room, OpCode.FIGHT, FightCode.DEAL_BRO, dto);
                   
                        if (remainCardList.Count == 0)
                        {
                            GameOver(userId, room);
                        }
                        else
                        {
                            Turn(room);
                        }
                    }
                });
        }

        
        private void GameOver(int userId, FightRoom room)
        {
            int winIdentity = room.GetPlayerIdentity(userId);
            int winBean = room.Multiple * 1000;
            // add wincount
            List<int> winUIds = room.GetSameIdentityUIds(winIdentity);
            for (int i = 0; i < winUIds.Count; i++)
            {
                UserModel um = userCache.GetModelById(winUIds[i]);
                um.WinCount++;
                um.Bean += winBean;
                um.Exp += 100;
                int maxExp = um.Lv * 100;
                while (maxExp <= um.Exp)
                {
                    um.Lv++;
                    um.Exp -= maxExp;
                    maxExp = um.Lv * 100;
                }
                userCache.Update(um);
            }
            // add losscount
            List<int> loseUIds = room.GetDifferentIdentityUIds(winIdentity);
            for (int i = 0; i < loseUIds.Count; i++)
            {
                UserModel um = userCache.GetModelById(loseUIds[i]);
                um.LoseCount++;
                um.Bean -= winBean;
                um.Exp += 10;
                int maxExp = um.Lv * 100;
                while (maxExp <= um.Exp)
                {
                    um.Lv++;
                    um.Exp -= maxExp;
                    maxExp = um.Lv * 100;
                }
                userCache.Update(um);
            }
            // add runcount
            for (int i = 0; i < room.LeaveUIdList.Count; i++)
            {
                UserModel um = userCache.GetModelById(room.LeaveUIdList[i]);
                um.RunCount++;
                um.Bean -= (winBean) * 3;
                um.Bean += 0;
                int maxExp = um.Lv * 100;
                while (maxExp <= um.Exp)
                {
                    um.Lv++;
                    um.Exp -= maxExp;
                    maxExp = um.Lv * 100;
                }
                userCache.Update(um);
            }

            
            OverDto dto = new OverDto();
            dto.WinIdentity = winIdentity;
            dto.WinUIdList = winUIds;
            dto.BeanCount = winBean;
            Broadcast(room, OpCode.FIGHT, FightCode.OVER_BRO, dto);

            // destroy romm data in cache layer
            fightCache.Destroy(room);
        }

        
        private void Turn(FightRoom room)
        {
            int nextUId = room.Turn();
            if (room.IsOffline(nextUId) == true)
            {
                Turn(room);
            }
            else
            {
                // player is not offline
                Broadcast(room, OpCode.FIGHT, FightCode.TURN_DEAL_BRO, nextUId);
            }
        }

        
        private void GrabLandlord(ClientPeer client, bool result)
        {
            SingleThread.Instance.SingleExecute(
                delegate ()
                {
                    if (userCache.IsOnline(client) == false)
                        return;
                    // ensure player is online fighting
                    int userId = userCache.GetId(client);
                    FightRoom room = fightCache.GetRoomByUId(userId);
                    // whether player grabs
                    if (result == true)
                    {
                        room.SetLandlord(userId);
                        GrabDto dto = new GrabDto(userId, room.TableCardList, room.GetUserCards(userId));
                        Broadcast(room, OpCode.FIGHT, FightCode.GRAB_LANDLORD_BRO, dto);
                        
                        Broadcast(room, OpCode.FIGHT, FightCode.TURN_DEAL_BRO, userId);
                    }
                    else
                    {
                        int nextUId = room.GetNextUId(userId);
                        Broadcast(room, OpCode.FIGHT, FightCode.TURN_GRAB_BRO, nextUId);
                    }

                });
        }

        public void StartFight(List<int> uidList)
        {
            SingleThread.Instance.SingleExecute(
                delegate ()
                {
                    // create room
                    FightRoom room = fightCache.Create(uidList);
                    room.InitPlayerCards();
                    room.Sort();
                    // tell every server his cards
                    foreach (int uid in uidList)
                    {
                        ClientPeer client = userCache.GetClientPeer(uid);
                        List<CardDto> cardList = room.GetUserCards(uid);
                        client.Send(OpCode.FIGHT, FightCode.GET_CARD_SRES, cardList);
                    }
                    
                    int firstUserId = room.GetFirstUId();

                    Broadcast(room, OpCode.FIGHT, FightCode.TURN_GRAB_BRO, firstUserId, null);
                });
        }

    
        private void Broadcast(FightRoom room, int opCode, int subCode, object value, ClientPeer exClient = null)
        {
            SocketMsg msg = new SocketMsg(opCode, subCode, value);
            byte[] data = EncodeTool.EncodeMsg(msg);
            byte[] packet = EncodeTool.EncodePacket(data);

            foreach (var player in room.PlayerList)
            {
                if (userCache.IsOnline(player.UserId))
                {
                    ClientPeer client = userCache.GetClientPeer(player.UserId);
                    if (client == exClient)
                        continue;
                    client.Send(packet);
                }
            }
        }

    }
}
