using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProtocol.Code
{
    public class UserCode
    {
        // get info
        public const int GET_INFO_CREQ = 0;
        public const int GET_INFO_SRES = 1;

        // create character
        public const int CREATE_CREQ = 2;
        public const int CREATE_SRES = 3;

        // character online
        public const int ONLINE_CREQ = 4;
        public const int ONLINE_SRES = 5;
    }
}
