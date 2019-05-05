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
        private static SingleThread instance = null;
        public static SingleThread Instance
        {
            get
            {
                lock (o)
                {
                    if (instance == null)
                        instance = new SingleThread();
                    return instance;
                }
            }
        }

        private static object o = 1;

        /// <summary>
        /// mutex lock
        /// </summary>
        public Mutex mutex;

        private SingleThread()
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
