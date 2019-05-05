using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    
    public class UserModel
    {
        public int Id;
        public string Name;
        public int Bean;
        public int WinCount;
        public int LoseCount;
        public int RunCount;
        public int Lv;
        public int Exp;


        public int AccountId;


        public UserModel(int id, string name, int accountId)
        {
            this.Id = id;
            this.Name = name;
            this.Bean = 10000;
            this.WinCount = 0;
            this.LoseCount = 0;
            this.RunCount = 0;
            this.Lv = 1;
            this.Exp = 0;
            this.AccountId = accountId;
        }

    }
}
