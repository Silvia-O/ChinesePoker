using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProtocol.Dto
{
    [Serializable]
    public class AccountDto
    {
        public string Acc;
        public string Pwd;

        public AccountDto()
        {

        }
        public AccountDto(string acc, string pwd)
        {
            this.Acc = acc;
            this.Pwd = pwd;
        }
    }
}
