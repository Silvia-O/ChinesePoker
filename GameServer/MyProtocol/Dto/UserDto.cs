using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProtocol.Dto
{
    [Serializable]
    public class UserDto
    {
        public int Id;
        public string Name;
        public int Bean;
        public int WinCount;
        public int LoseCount;
        public int RunCount;
        public int Lv;
        public int Exp;

        public UserDto()
        {

        }

        public UserDto(int id, string name, int bean, int winCount, int loseCount, int runCount, int lv, int exp)
        {
            this.Id = id;
            this.Name = name;
            this.Bean = bean;
            this.WinCount = winCount;
            this.LoseCount = loseCount;
            this.RunCount = runCount;
            this.Lv = lv;
            this.Exp = exp;
        }
    }
}
