using MyServer.Util.Concurrent;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MyServer.Util.Timera
{
    public class TimerHandler
    {
        /// <summary>
        /// singleton: one class, one object instance
        /// </summary>
        private static TimerHandler instance = null;
        public static TimerHandler Instance
        {
            get
            {
                lock (instance)
                {
                    if (instance == null)
                        instance = new TimerHandler();
                    return instance;
                }
                
            }
        }

        private Timer timer;

        /// <summary>
        /// mapping of id to model
        /// </summary>
        private ConcurrentDictionary<int, TimerModel> idModelDict = new ConcurrentDictionary<int, TimerModel>();

        /// <summary>
        /// list of timer tasks to be deleted
        /// </summary>
        private List<int> rmIdList = new List<int>();

        /// <summary>
        /// timer task id
        /// </summary>
        private ConcurrentInt id = new ConcurrentInt(-1);

        public TimerHandler()
        {
            timer = new Timer(10);
            timer.Elapsed += Timer_Elapsed;
        }

        /// <summary>
        /// triggered at every interval
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (rmIdList)
            {
                TimerModel tm = null;
                foreach (var id in rmIdList)
                {
                    idModelDict.TryRemove(id, out tm);
                }
                rmIdList.Clear();
            }
           
            foreach(var model in idModelDict.Values)
            {
                if(model.Time <= DateTime.Now.Ticks)
                    model.Run();
            }
        }

        /// <summary>
        /// add timer task: trigger time
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="te"></param>
        public void AddTimerEvent(DateTime dateTime, TimeEvent te)
        {
            long delayTime = dateTime.Ticks - DateTime.Now.Ticks;
            if (delayTime <= 0)
                return;
            AddTimerEvent(delayTime, te);
        }

        /// <summary>
        /// add timer task: delay time
        /// </summary>
        /// <param name="delayTime">ms</param>
        /// <param name="te"></param>
        public void AddTimerEvent(long delayTime, TimeEvent te)
        {
            TimerModel timermodel = new TimerModel(id.IncreaseGet(), DateTime.Now.Ticks + delayTime, te);
            idModelDict.TryAdd(timermodel.Id, timermodel);
        }

    }
}
