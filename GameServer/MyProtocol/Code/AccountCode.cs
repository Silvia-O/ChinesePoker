using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProtocol.Code
{
    public class AccountCode
    {
        // param: AccountDto
        public const int REGISTER_CREQ = 0;
        public const int REGISTER_SRES = 1;

        public const int LOGIN_CREQ = 2;
        public const int LOGIN_SRES = 3;
    }
}
