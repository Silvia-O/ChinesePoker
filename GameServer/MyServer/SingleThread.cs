using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyServer
{
    public delegate void ExecuteDelegate();

    public class SingleThread
    {
        /// <summary>
        /// mutex lock
        /// </summary>
        public Mutex mutex;

        public SingleThread()
        {
            mutex = new Mutex();
        }

        /// <summary>
        /// single thread executing method
        /// </summary>
        /// <param name="executeDelegate"></param>
        public void SingleExecute(ExecuteDelegate executeDelegate)
        {
            lock (this)
            {
                mutex.WaitOne();
                executeDelegate();
                mutex.ReleaseMutex();
            }
        }
    }
}
