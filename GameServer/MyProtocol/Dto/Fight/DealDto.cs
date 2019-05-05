using MyProtocol.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProtocol.Dto.Fight
{
    [Serializable]
    public class DealDto
    {
     
        public List<CardDto> SelectCardList;
      
        public int Length;
    
        public int Weight;
    
        public int Type;

       
        public int UserId;
    
        public bool IsRegular;
    
        public List<CardDto> RemainCardList;

        public DealDto()
        {

        }

        public DealDto(List<CardDto> cardList, int uid)
        {
            this.SelectCardList = cardList;
            this.Length = cardList.Count;
            this.Type = CardType.GetCardType(cardList);
            this.Weight = CardWeight.GetWeight(cardList, this.Type);
            this.UserId = uid;
            this.IsRegular = (this.Type != CardType.NONE);
            this.RemainCardList = new List<CardDto>();
        }
    }
}
