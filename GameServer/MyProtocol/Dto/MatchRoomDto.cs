using MyProtocol.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProtocol.Dto
{
    [Serializable]
    public class MatchRoomDto
    {
        
        public Dictionary<int, UserDto> UIdUserDict;

        
        public List<int> ReadyUIdList;

        // players entering oder
        public List<int> UIdList;

        public MatchRoomDto()
        {
            this.UIdUserDict = new Dictionary<int, UserDto>();
            this.ReadyUIdList = new List<int>();
            this.UIdList = new List<int>();
        }

        public void Add(UserDto newUser)
        {
            this.UIdUserDict.Add(newUser.Id, newUser);
            this.UIdList.Add(newUser.Id);
        }

        public void Leave(int userId)
        {
            this.UIdUserDict.Remove(userId);
            this.UIdList.Remove(userId);
        }

        public void Ready(int userId)
        {
            this.ReadyUIdList.Add(userId);
        }

        public int LeftId;
        public int RightId;

        /// <summary>
        /// reset seat
        /// </summary>
        /// <param name="myUserId"></param>
        public void ResetPosition(int myUserId)
        {
            LeftId = -1;
            RightId = -1;

            // 1
            if (UIdList.Count == 1)
            {
                
            }
            // 2
            else if (UIdList.Count == 2)
            {
                // x a
                if(UIdList[0] == myUserId)
                {
                    RightId = UIdList[1];
                }
                // a x
                if (UIdList[1] == myUserId)
                {
                    LeftId = UIdList[0];
                }
            }
            // 3.
            else if(UIdList.Count == 3)
            {
                // x a b
                if (UIdList[0] == myUserId)
                {
                    LeftId = UIdList[2];
                    RightId = UIdList[1];
                }
                // a x b
                if (UIdList[1] == myUserId)
                {
                    LeftId = UIdList[0];
                    RightId = UIdList[2];
                }
                // a b x
                if (UIdList[2] == myUserId)
                {
                    LeftId = UIdList[1];
                    RightId = UIdList[0];
                }
            }
        }

    }
}
