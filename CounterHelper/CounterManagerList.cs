using System;
using System.Collections.Generic;
using System.Threading;

namespace CounterHelper
{
    public static class CounterManagerList
    {
        public static List<CounterManager> counterManagerList = new List<CounterManager>();

        private static Thread[] threads;

        public static void RunCounterListConsoleAsync()
        {
            threads = new Thread[counterManagerList.Count];

            for (var i = 0; i < counterManagerList.Count; i++)
            {
                var i1 = i;
                threads[i] = new Thread(() => Helper.PrintCounter(counterManagerList[i1]));
                threads[i].Start();
            }

        }
        public static void RunCounterListAsync()
        {
            threads = new Thread[counterManagerList.Count];

            for (var i = 0; i < counterManagerList.Count; i++)
            {
                var i1 = i;
                threads[i] = new Thread(() => counterManagerList[i1].GetCounterValueForever());
                threads[i].Start();
            }
        }

        public static void StopAllCounters()
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
    }


}