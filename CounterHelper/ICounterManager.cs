using System.Diagnostics;

namespace CounterHelper
{
    public interface ICounterManager
    {
        PerformanceCounter Counter { get; set; }
        Options options { get; set; }
        bool CounterReady();
        string GetCounterValue();
        void GetCounterValueForever();
    }
}