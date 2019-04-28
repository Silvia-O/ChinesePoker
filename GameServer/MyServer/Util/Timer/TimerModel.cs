using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Util.Timera
{
    /// <summary>
    /// tiggered when time is up
    /// </summary>
    public delegate void TimeEvent();

    public class TimerModel
    {
        public int Id;

        /// <summary>
        /// execution time of task
        /// </summary>
        public long Time;

        public TimeEvent timeEvent;

        public TimerModel(int id, long time, TimeEvent te)
        {
            this.Id = id;
            this.Time = time;
            this.timeEvent = te;
        }

        public void Run()
        {
            timeEvent();
        }
    }
}
