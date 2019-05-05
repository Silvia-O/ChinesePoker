using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProtocol.Dto.Fight
{
    [Serializable]
    public class PlayerDto
    {
        public int UserId;
        public int Identity;
        public List<CardDto> CardList;

        public PlayerDto(int userId)
        {
            this.Identity = MyProtocol.Constant.Identity.FARMER;
            this.UserId = userId;
            this.CardList = new List<CardDto>();
        }

 
        public bool HasCard
        {
            get { return CardList.Count != 0; }
        }

        public int CardCount
        {
            get { return CardList.Count; }

        }


        public void Add(CardDto card)
        {
            CardList.Add(card);
        }

 
        public void Remove(CardDto card)
        {
            CardList.Remove(card);
        }
    }
}
