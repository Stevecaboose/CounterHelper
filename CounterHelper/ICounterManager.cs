using System.Diagnostics;

namespace CounterHelper
{
    public interface ICounterManager
    {
        PerformanceCounter Counter { get; set; }

        Options options { get; set; }

        string GetCounterValue();

        void GetCounterValueForever();

        void StartCounter();

        void StopCounter();

        bool GetStartStopState();

        bool SetCounterAsReady();
    }
}