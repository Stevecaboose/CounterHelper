using System;
using System.Diagnostics;
using System.Threading;

namespace CounterHelper
{
	public class CounterManager : ICounterManager
	{

		private PerformanceCounter _counter;
		private readonly CounterSample initalCounterSample;
		public string _firstValue;
		public string _lastValue;
		private int _iteration = 1000;
		private string _name;
		public DateTime _startTime = DateTime.Now;
		public DateTime _lastUpdate;
		public bool cancelCounter = false;
		public Guid guid = Guid.NewGuid();
		public string counterHelp;

		public Options _options;

		public PerformanceCounter Counter
		{
			get => _counter;
			set => _counter = value;
		}

		public Options options
		{
			get => _options;
			set => _options = value;

		}

		public CounterManager(Options options)
		{
			if (_options == null)
			{
				_options = options;
			}

			_counter = new PerformanceCounter();

			if (!string.IsNullOrEmpty(_options.CategoryName))
			{
				_counter.CategoryName = _options.CategoryName;
			}

			if (!string.IsNullOrEmpty(_options.CounterName))
			{
				_counter.CounterName = _options.CounterName;
			}

			if (!string.IsNullOrEmpty(_options.InstanceName))
			{
				_counter.InstanceName = _options.InstanceName;
			}

			_counter.MachineName = !string.IsNullOrEmpty(_options.MachineName) ? _options.MachineName : ".";

			_counter.ReadOnly = _options.ReadOnly ?? true;

			if (_options.IterationLength != null && _options.IterationLength > 0)
			{
				_iteration = options.IterationLength ?? 1000;
			}

			if (!string.IsNullOrEmpty(_options.Name))
			{
				_name = _options.Name;
			}

			try
			{
				StartCounter();
				_firstValue = GetCounterValue();
				initalCounterSample = _counter.NextSample();
				counterHelp = _counter.CounterHelp;
				_lastUpdate = DateTime.Now;
				Thread.Sleep(5000);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

		}

		public bool CounterReady()
		{
			Thread.Sleep(_iteration);
			return true;
		}

		public string GetCounterValue()
		{
			if (cancelCounter) return null;
			if (initalCounterSample.CounterType == PerformanceCounterType.ElapsedTime)
			{
				_firstValue = _lastValue;
				var counterSample = _counter.NextSample();
				_lastValue = DateTime.Now.Subtract(DateTime.FromFileTimeUtc(counterSample.RawValue).ToLocalTime())
					.ToString();
			}
			else
			{
				_firstValue = _lastValue;
				_lastValue = _counter.NextValue().ToString();
			}

			_lastUpdate = DateTime.Now;
			return _lastValue;

		}

		public void StartCounter()
		{
			cancelCounter = false;
		}

		public void StopCounter()
		{
			cancelCounter = true;
		}

		public void GetCounterValueForever()
		{
			Console.WriteLine($"The counter manager is now collecting data on {Environment.MachineName} for counter: {Counter.CounterName}");
			if (_iteration < 1000) _iteration = 1000;
			while (!cancelCounter)
			{
				Thread.Sleep(_iteration);
				GetCounterValue();
			}
		}

	}
}