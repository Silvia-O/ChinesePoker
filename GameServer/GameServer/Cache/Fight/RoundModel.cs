using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Cache.Fight
{
    public class RoundModel
    {
        /// <summary>
        /// current dealing user id
        /// </summary>
        public int CurrentUId { get; set; }

        /// <summary>
        /// biggest dealing user in this round
        /// </summary>
        public int BiggestUId { get; set; }

        /// <summary>
        /// length of last card list
        /// </summary>
        public int LastLength { get; set; }

        /// <summary>
        /// weight of last card list
        /// </summary>
        public int LastWeight { get; set; }

        /// <summary>
        /// typt of last card list
        /// </summary>
        public int LastCardType { get; set; }


        public RoundModel()
        {
            this.CurrentUId = -1;
            this.BiggestUId = -1;
            this.LastLength = -1;
            this.LastWeight = -1;
            this.LastCardType = -1;
        }

        public void Init()
        {
            this.CurrentUId = -1;
            this.BiggestUId = -1;
            this.LastLength = -1;
            this.LastWeight = -1;
            this.LastCardType = -1;
        }

        
        public void Start(int userId)
        {
            this.CurrentUId = userId;
            this.BiggestUId = userId;
        }

        /// <summary>
        /// change dealing player
        /// </summary>
        /// <param name="length"></param>
        /// <param name="type"></param>
        /// <param name="weight"></param>
        /// <param name="userId"></param>
        public void Change(int length,int type,int weight,int userId)
        {
            this.BiggestUId = userId;
            this.LastLength = length;
            this.LastCardType = type;
            this.LastWeight = weight;
        }

        /// <summary>
        /// turn to dealing player
        /// </summary>
        /// <param name="userId"></param>
        public void Turn(int userId)
        {
            this.CurrentUId = userId;
        }

    }
}
