using MyProtocol.Constant;
using MyProtocol.Dto.Fight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Cache.Fight
{
    public class LibraryModel
    {
        public Queue<CardDto> CardQueue { get; set; }

        public LibraryModel()
        {
            Create();
            Shuffle();
        }

        public void Init()
        {
            Create();
            Shuffle();
        }

        private void Create()
        {
            CardQueue = new Queue<CardDto>();
            for (int color = CardColor.CLUB; color <= CardColor.DIAMOND; color++)
            {
                for (int weight = CardWeight.THREE; weight <= CardWeight.TWO; weight++)
                {
                    string cardName = CardColor.GetString(color) + CardWeight.GetString(weight);
                    CardDto card = new CardDto(cardName, color, weight);
             
                    CardQueue.Enqueue(card);
                }
            }
            CardDto sJoker = new CardDto("SJoker", CardColor.NONE, CardWeight.SJOKER);
            CardDto lJoker = new CardDto("LJoker", CardColor.NONE, CardWeight.LJOKER);
            CardQueue.Enqueue(sJoker);
            CardQueue.Enqueue(lJoker);
        }

        
        private void Shuffle()
        {
            List<CardDto> newList = new List<CardDto>();
            Random r = new Random();
            foreach (CardDto card in CardQueue)
            {
                int index = r.Next(0, newList.Count + 1);
                newList.Insert(index, card);
            }
            CardQueue.Clear();
            foreach (CardDto card in newList)
            {
                CardQueue.Enqueue(card);
            }
        }

        public CardDto Deal()
        {
            return CardQueue.Dequeue();
        }

    }
}
