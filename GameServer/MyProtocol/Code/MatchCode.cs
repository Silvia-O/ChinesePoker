using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProtocol.Code
{
    public class MatchCode
    {
        // enter match 
        public const int ENTER_CREQ = 0;
        public const int ENTER_SRES = 1;
        public const int ENTER_BRO = 2;

        // leave match
        public const int LEAVE_CREQ = 3;
        public const int LEAVE_BRO = 4;

        // get ready
        public const int READY_CREQ = 5;
        public const int READY_BRO = 6;

        // get started
        public const int START_BRO = 7;
    }
}
