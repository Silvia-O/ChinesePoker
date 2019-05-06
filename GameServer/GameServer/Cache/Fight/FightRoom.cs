using MyServer;
using MyProtocol.Constant;
using MyProtocol.Dto.Fight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Cache.Fight
{
    public class FightRoom
    {
        
        public int Id { get; private set; }

       
        public List<PlayerDto> PlayerList { get; set; }
        
        public List<int> LeaveUIdList { get; set; }

        public LibraryModel libraryModel { get; set; }
        
        public List<CardDto> TableCardList { get; set; }
        
        public int Multiple { get; set; }
        
        public RoundModel roundModel { get; set; }

        
        public FightRoom(int id, List<int> uidList)
        {
            this.Id = id;
            this.PlayerList = new List<PlayerDto>();
            foreach (int uid in uidList)
            {
                PlayerDto player = new PlayerDto(uid);
                this.PlayerList.Add(player);
            }
            this.LeaveUIdList = new List<int>();
            this.libraryModel = new LibraryModel();
            this.TableCardList = new List<CardDto>();
            this.Multiple = 1;
            this.roundModel = new RoundModel();
        }

        public void Init(List<int> uidList)
        {
            foreach (int uid in uidList)
            {
                PlayerDto player = new PlayerDto(uid);
                this.PlayerList.Add(player);
            }
        }

        public bool IsOffline(int uid)
        {
            return LeaveUIdList.Contains(uid);
        }

        /// <summary>
        /// turn to deal
        /// </summary>
        public int Turn()
        {
            int currUId = roundModel.CurrentUId;
            int nextUId = GetNextUId(currUId);
            
            roundModel.CurrentUId = nextUId;
            return nextUId;
        }

        /// <summary>
        /// get next player to deal
        /// </summary>
        /// <param name="currUId"></param>
        /// <returns></returns>
        public int GetNextUId(int currUId)
        {
            for (int i = 0; i < PlayerList.Count; i++)
            {
                if (PlayerList[i].UserId == currUId)
                {
                    // 1 2 3
                    if (i == 2)
                        return PlayerList[0].UserId;
                    else
                        return PlayerList[i + 1].UserId;
                }
            }
            throw new Exception("No this dealer！");
        }

        /// <summary>
        /// deal card logic 
        /// </summary>
        /// <returns></returns>
        public bool DealCard(int type, int weight, int length, int userId, List<CardDto> cardList)
        {
            bool canDeal = false;

            if (type == roundModel.LastCardType && weight > roundModel.LastWeight)
            {
                if (type == CardType.STRAIGHT || type == CardType.DOUBLE_STRAIGHT || type == CardType.TRIPLE_STRAIGHT)
                {
                    if (length == roundModel.LastLength)
                    {
                        canDeal = true;
                    }
                }
                else
                {
                    canDeal = true;
                }
            }
            else if (type == CardType.BOOM && roundModel.LastCardType != CardType.BOOM)
            {
                canDeal = true;
            }
            else if (type == CardType.JOKER_BOOM)
            {
                canDeal = true;
            }
            else if (userId == roundModel.BiggestUId)
            {
                canDeal = true;
            }

            if (canDeal)
            {
                RemoveCards(userId, cardList);
                if (type == CardType.BOOM)
                {
                    this.Multiple *= 4;
                }
                else if (type == CardType.JOKER_BOOM)
                {
                    this.Multiple *= 8;
                }
                roundModel.Change(length, type, weight, userId);
            }

            return canDeal;
        }

        /// <summary>
        /// remove cards
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cardList"></param>
        private void RemoveCards(int userId, List<CardDto> cardList)
        {
            List<CardDto> currList = GetUserCards(userId);
            List<CardDto> list = new List<CardDto>();
            foreach (var select in cardList)
            {
                for (int i = currList.Count - 1; i >= 0; i--)
                {
                    if (currList[i].Name == select.Name)
                    {
                        list.Add(currList[i]);
                        break;
                    }
                }
            }
            foreach (var card in list)
                currList.Remove(card);
        }


        /// <summary>
        /// get user's cards
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<CardDto> GetUserCards(int userId)
        {
            foreach (PlayerDto player in PlayerList)
            {
                if (player.UserId == userId)
                    return player.CardList;
            }
            throw new Exception("No this player！");
        }

        /// <summary>
        /// init player cards
        /// </summary>
        public void InitPlayerCards()
        {
            // 17 * 3 = 51, 17 per player
            // 51 + 3 = 54, 3 for table cards
            for (int i = 0; i < 17; i++)
            {
                CardDto card = libraryModel.Deal();
                PlayerList[0].Add(card);
            }
            for (int i = 0; i < 17; i++)
            {
                CardDto card = libraryModel.Deal();
                PlayerList[1].Add(card);
            }
            for (int i = 0; i < 17; i++)
            {
                CardDto card = libraryModel.Deal();
                PlayerList[2].Add(card);
            }
            
            for (int i = 0; i < 3; i++)
            {
                CardDto card = libraryModel.Deal();
                TableCardList.Add(card);
            }
        }

        /// <summary>
        /// set landlord
        /// </summary>
        public void SetLandlord(int userId)
        {
            foreach (PlayerDto player in PlayerList)
            {
                if (player.UserId == userId)
                {
                    player.Identity = Identity.LANDLORD;
                    // add table cards to landlord
                    for (int i = 0; i < TableCardList.Count; i++)
                    {
                        player.Add(TableCardList[i]);
                    }
                    // resort
                    this.Sort();
                    // begin game round
                    roundModel.Start(userId);
                }
            }
        }

        /// <summary>
        /// get player model data
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PlayerDto GetPlayerModel(int userId)
        {
            foreach (PlayerDto player in PlayerList)
            {
                if (player.UserId == userId)
                {
                    return player;
                }
            }
            throw new Exception("No this player!");
            //return null;
        }

        /// <summary>
        /// get player identity
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int GetPlayerIdentity(int userId)
        {
            return GetPlayerModel(userId).Identity;
            throw new Exception("No this player!");
        }

        /// <summary>
        /// get same identity user id
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public List<int> GetSameIdentityUIds(int identity)
        {
            List<int> uids = new List<int>();
            foreach (PlayerDto player in PlayerList)
            {
                if (player.Identity == identity)
                {
                    uids.Add(player.UserId);
                }
            }
            return uids;
        }

        /// <summary>
        /// get different identity user id
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public List<int> GetDifferentIdentityUIds(int identity)
        {
            List<int> uids = new List<int>();
            foreach (PlayerDto player in PlayerList)
            {
                if (player.Identity != identity)
                {
                    uids.Add(player.UserId);
                }
            }
            return uids;
        }

        /// <summary>
        /// get first user id in this room
        /// </summary>
        /// <returns></returns>
        public int GetFirstUId()
        {
            return PlayerList[0].UserId;
        }

        /// <summary>
        /// sort cards
        /// </summary>
        /// <param name="cardList"></param>
        /// <param name="asc"></param>
        private void SortCard(List<CardDto> cardList, bool asc = true)//asc des
        {
            cardList.Sort(
                delegate (CardDto a, CardDto b)
                {
                    if (asc)
                        return a.Weight.CompareTo(b.Weight);
                    else
                        return a.Weight.CompareTo(b.Weight) * -1;
                });
        }

        /// <summary>
        /// default ascend
        /// </summary>
        /// <param name="asc"></param>
        public void Sort(bool asc = true)
        {
            SortCard(PlayerList[0].CardList, asc);
            SortCard(PlayerList[1].CardList, asc);
            SortCard(PlayerList[2].CardList, asc);
            SortCard(TableCardList, asc);
        }

    }
}
