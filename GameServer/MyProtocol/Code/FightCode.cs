using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProtocol.Code
{
    public class FightCode
    {
        public const int GRAB_LANDLORD_CREQ = 0;
        public const int GRAB_LANDLORD_BRO = 1;
        public const int TURN_GRAB_BRO = 2;

        public const int DEAL_CREQ = 3;
        public const int DEAL_SRES = 4;
        public const int DEAL_BRO = 5;

        public const int PASS_CREQ = 6;
        public const int PASS_SRES = 7;

        public const int TURN_DEAL_BRO = 8;

        public const int LEAVE_BRO = 9;

        public const int OVER_BRO = 10;

        public const int GET_CARD_SRES = 11;
    }
}
