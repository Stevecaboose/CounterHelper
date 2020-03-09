using System;
using System.Collections.Generic;
using System.Threading;

namespace CounterHelper
{
    /// <summary>
    /// Used to manage the multiple Counters
    /// </summary>
    public static class CounterManagerList
    {
		#region Public Variables

		public static List<CounterManager> counterManagerList = new List<CounterManager>();

        private static Thread[] threads;

		#endregion

		#region Public Methods

        /// <summary>
        /// Builds the needed threads to have each counter run on its own thread
        /// </summary>
        public static void RunCounterListAsync()
        {
            threads = new Thread[counterManagerList.Count];

            for (var i = 0; i < counterManagerList.Count; i++)
            {
                var i1 = i;
                threads[i] = new Thread(() => counterManagerList[i1].StartCounter());
                threads[i].Start();
            }
        }

        /// <summary>
        /// Stops AND clears all counters regardless if they are paused
        /// </summary>
        public static void ForceStopAllCounters()
        {
            try
            {
                foreach (var thread in threads)
                {
                    thread.Abort();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            foreach (var counter in counterManagerList)
            {
                counter.Counter.Dispose();
            }
            counterManagerList.Clear();
        }

        #endregion
    }
}