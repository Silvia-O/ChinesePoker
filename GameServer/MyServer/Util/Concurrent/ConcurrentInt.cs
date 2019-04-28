using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Util.Concurrent
{
    /// <summary>
    /// thread-safe int 
    /// </summary>
    public class ConcurrentInt
    {
        private int Value;

        public ConcurrentInt(int value)
        {
            this.Value = value;
        }

        public int IncreaseGet()
        {
            lock (this)
            {
                Value++;
                return Value;
            }
        }

        public int DecreaseGet()
        {
            lock (this)
            {
                Value--;
                return Value;
            }
        }

        public int Get()
        {
            return Value;
        }

    }
}
