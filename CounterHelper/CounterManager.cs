using System;
using System.Diagnostics;
using System.Threading;

namespace CounterHelper
{
	/// <summary>
	/// Builds an object that contains a single counter. The CounterManager can then process a counter and
	/// perform actions on that counter
	/// </summary>
	public class CounterManager : ICounterManager
	{
		#region Private variables

		private CounterSample initalCounterSample;
		private int _iteration = 1000;
		private string _name;
		private bool cancelCounter = false;
		private Guid _guid = Guid.NewGuid();

		#endregion

		#region Public Properties

		public PerformanceCounter Counter { get; set; }

		public Guid GUID
		{
			get => _guid;
		}

		public DateTime StartTime { get; set; } = DateTime.Now;

		public DateTime LastUpdate { get; set; }

		public string FirstValue { get; set; }

		public string LastValue { get; set; }

		public string CounterHelp { get; set; }

		public Options options { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Builds a CounterManager object with a single Options object
		/// </summary>
		/// <param name="options"></param>
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
				Init();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

		}

		public CounterManager(PerformanceCounter performanceCounter)
		{
			// Build options object

			Counter = performanceCounter;

			// Set iteration length to default 1 second
			_iteration = 1000;

			try
			{
				Init();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Puts the counter in a ready state. There is an initial sleep in order to have the next counter value ready
		/// by the time it is ready to be used.
		/// </summary>
		/// <returns></returns>
		public bool CounterReady()
		{
			Thread.Sleep(_iteration);
			return true;
		}

		/// <summary>
		/// Gets the next value of the counter
		/// </summary>
		/// <returns>string of the value. This is a string because of the variating formats that might be
		/// returned by the counter</returns>
		public string GetCounterValue()
		{
			if (cancelCounter) return null;

			// This is if we are dealing with a time
			// The time must be calculated and therefore can't be returned by the Value
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

		/// <summary>
		/// Sets the state of the Counter in a non-cancelled state
		/// </summary>
		public void StartCounter()
		{
			cancelCounter = false;
		}

		/// <summary>
		/// Sets the state of the Counter in a cancelled state
		/// It will pause duiring this time
		/// </summary>
		public void StopCounter()
		{
			cancelCounter = true;
		}

		/// <summary>
		/// Starts to run the Counter until interrupted.
		/// </summary>
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

		#endregion

		#region Private methods

		/// <summary>
		/// Gets the counter ready for use
		/// </summary>
		private void Init()
		{
			StartCounter();
			// Getting the first value is necessary in order to have an original value to compare to
			FirstValue = GetCounterValue();
			initalCounterSample = Counter.NextSample();
			CounterHelp = Counter.CounterHelp;
			LastUpdate = DateTime.Now;
			Thread.Sleep(5000);
		}

		#endregion
	}
}