using System;
using System.Diagnostics;
using System.Threading;

namespace CounterHelper
{
	public class CounterManager : ICounterManager
	{
		private readonly CounterSample initalCounterSample;
		private int _iteration = 1000;
		private readonly string _name;
		private bool cancelCounter = false;
		private Guid _guid = Guid.NewGuid();

		public PerformanceCounter Counter { get; set; }

		public Guid GUID
		{
			get => _guid;
		}

		public DateTime StartTime { get; set; } = DateTime.Now;

		public DateTime LastUpdate { get; set; }

		public string FirstValue { get; set; }

		public string LastValue { get; set; }

		public string CounterHelp { get; }

		public Options options { get; set; }

		public CounterManager(Options options)
		{
			if (this.options == null)
			{
				this.options = options;
			}

			Counter = new PerformanceCounter();

			if (!string.IsNullOrEmpty(this.options.CategoryName))
			{
				Counter.CategoryName = this.options.CategoryName;
			}

			if (!string.IsNullOrEmpty(this.options.CounterName))
			{
				Counter.CounterName = this.options.CounterName;
			}

			if (!string.IsNullOrEmpty(this.options.InstanceName))
			{
				Counter.InstanceName = this.options.InstanceName;
			}

			Counter.MachineName = !string.IsNullOrEmpty(this.options.MachineName) ? this.options.MachineName : ".";

			Counter.ReadOnly = this.options.ReadOnly ?? true;

			if (this.options.IterationLength != null && this.options.IterationLength > 0)
			{
				_iteration = options.IterationLength ?? 1000;
			}

			if (!string.IsNullOrEmpty(this.options.Name))
			{
				_name = this.options.Name;
			}

			try
			{
				StartCounter();
				FirstValue = GetCounterValue();
				initalCounterSample = Counter.NextSample();
				CounterHelp = Counter.CounterHelp;
				LastUpdate = DateTime.Now;
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
				FirstValue = LastValue;
				var counterSample = Counter.NextSample();
				LastValue = DateTime.Now.Subtract(DateTime.FromFileTimeUtc(counterSample.RawValue).ToLocalTime())
					.ToString();
			}
			else
			{
				FirstValue = LastValue;
				LastValue = Counter.NextValue().ToString();
			}

			LastUpdate = DateTime.Now;
			return LastValue;

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