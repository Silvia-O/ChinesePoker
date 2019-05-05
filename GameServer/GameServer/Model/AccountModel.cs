using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    public class AccountModel
    {
        public int Id;
        public string Acc;
        public string Pwd;

        public AccountModel(int id, string acc, string pwd)
        {
            this.Id = id;
            this.Acc = acc;
            this.Pwd = pwd;
        }
    }
}
