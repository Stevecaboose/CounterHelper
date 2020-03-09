using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace CounterHelper
{
	/// <summary>
	/// Static class that contains various useful methods
	/// </summary>
	public static class Helper
	{
		#region Public methods

		/// <summary>
		/// Prints all of the counters that are available to the specific machine it is being run on
		/// </summary>
		/// <param name="categoryFilter"></param>
		public static void PrintAllAvailableCounters(string categoryFilter)
		{
			var categories = PerformanceCounterCategory.GetCategories();
			foreach (var cat in categories)
			{
				if (!string.IsNullOrEmpty(categoryFilter))
				{
					if (!cat.CategoryName.Contains(categoryFilter)) continue;
				}
				Console.WriteLine("Category {0}", cat.CategoryName);
				try
				{
					var instances = cat.GetInstanceNames();
					if (instances.Length > 0)
					{
						foreach (var instance in instances)
						{
							foreach (var counter in cat.GetCounters(instance))
							{
								Console.WriteLine("\tCounter Name {0} [{1}]", counter.CounterName, instance);
							}
						}
					}
					else
					{
						foreach (var counter in cat.GetCounters())
						{
							Console.WriteLine("\tCounter Name {0}", counter.CounterName);
						}
					}
				}
				catch (Exception e)
				{
                    Console.WriteLine(e);
				}
			}
		}

		/// <summary>
		/// Gets a snapshot of all the counters that are running and builds a CounterData list based on each of the
		/// Counter's values
		/// </summary>
		/// <returns>List of CounterData objects</returns>
		public static List<CounterData> GetCounterData()
		{
			return CounterManagerList.counterManagerList.Select(counterManager => new CounterData()
			{
				GUID = counterManager.GUID.ToString(),
				Name = counterManager.options.Name,
				StartTime = counterManager.StartTime,
				LastUpdate = counterManager.LastUpdate,
				FirstValue = counterManager.FirstValue,
				LastValue = counterManager.LastValue,
				CategoryName = counterManager.options.CategoryName,
				CounterName = counterManager.options.CounterName,
				InstanceName = counterManager.options.InstanceName,
				MachineName = counterManager.options.MachineName,
				ReadOnly = counterManager.options.ReadOnly,
				IterationLength = counterManager.options.IterationLength,
				Units = counterManager.options.Units,
				CounterHelp = counterManager.CounterHelp
			})
				.ToList();
		}

		/// <summary>
		/// Gets the instance for a particular process
		/// </summary>
		/// <param name="processId">Process value</param>
		/// <returns></returns>
		public static string GetInstanceNameForProcessId(int processId)
		{
			var process = Process.GetProcessById(processId);
			var processName = Path.GetFileNameWithoutExtension(process.ProcessName);

			var cat = new PerformanceCounterCategory("Process");
			var instances = cat.GetInstanceNames()
				.Where(inst => inst.StartsWith(processName))
				.ToArray();

			foreach (var instance in instances)
			{
				using (PerformanceCounter cnt = new PerformanceCounter("Process",
					"ID Process", instance, true))
				{
					var val = (int)cnt.RawValue;
					if (val == processId)
					{
						return instance;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Creates a new Counter based on the process and the Windows Performance Monitor Counter Name
		/// </summary>
		/// <param name="processId"> Process value</param>
		/// <param name="processCounterName">Windows Performance Monitor Counter Name. 
		/// Default value = "% Processor Time"</param>
		/// <returns></returns>
		public static PerformanceCounter GetPerformanceCounterForProcessId(int processId, string processCounterName = "% Processor Time")
		{
			string instance = GetInstanceNameForProcessId(processId);
			return string.IsNullOrEmpty(instance) ? null : new PerformanceCounter("Process", processCounterName, instance);
		}

		/// <summary>
		/// Prints only the option name, counter name and counter value
		/// </summary>
		/// <param name="counter">Uses a CounterManager object to get the counter data</param>
		public static void PrintMinimalCounter(CounterManager counter)
		{
			string counterLine;

			while (true)
			{
				try
				{
					if (counter.CounterReady() && string.IsNullOrEmpty(counter.options.Name))
					{
						counterLine = counter.options.CategoryName + " \\ " + counter.options.CounterName + " : " +
									  counter.GetCounterValue();
					}
					else
					{
						counterLine = counter.options.Name + ": " + counter.options.CategoryName + " \\ " +
									  counter.options.CounterName + " : " + counter.GetCounterValue();
					}

					Console.WriteLine(counterLine);
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
			}
		}

		/// <summary>
		/// Gets all the Counter categories based on the machine it is running on
		/// </summary>
		/// <returns></returns>
		public static PerformanceCounterCategory[] GetAllPerformanceCounterCategory()
		{
			try
			{
				var perfCats = PerformanceCounterCategory.GetCategories();
				foreach (var category in perfCats.OrderBy(c => c.CategoryName))
				{
					Console.WriteLine("Category Name: {0}", category.CategoryName);
				}
				return perfCats;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
			return null;
		}

		/// <summary>
		/// Gets all the counters from a list of categories
		/// </summary>
		/// <param name="performanceCounterCategories">List of PerformanceCounterCategory</param>
        public static void GetAllCountersForAllCategories(PerformanceCounterCategory[] performanceCounterCategories)
        {
            foreach (var category in performanceCounterCategories)
            {
                GetAllCountersForACategory(category.CategoryName);
            }
        }

		/// <summary>
		/// Gets all the counters from a single category
		/// </summary>
		/// <param name="categoryName">string of the Category Name</param>
		public static void GetAllCountersForACategory(string categoryName)
		{
			PerformanceCounterCategory[] perfCats = PerformanceCounterCategory.GetCategories();

			PerformanceCounterCategory cat = perfCats.FirstOrDefault(c => c.CategoryName == categoryName);
			if (cat != null)
			{
				Console.WriteLine("Category Name: {0}", cat.CategoryName);

				string[] instances = cat.GetInstanceNames();

				if (instances.Length == 0)
				{
					foreach (PerformanceCounter counter in cat.GetCounters())
					{
						Console.WriteLine("     Counter Name: {0}", counter.CounterName);
					}
				}
				else
				{
					foreach (string instance in instances)
					{
						Console.WriteLine("  Instance Name: {0}", instance);
						if (cat.InstanceExists(instance))
						{
							foreach (PerformanceCounter counter in cat.GetCounters(instance))
								Console.WriteLine("     Counter Name: {0}", counter.CounterName);
						}
					}
				}
			}
		}

		/// <summary>
		/// Tests one Performance Counter for 10 iterations
		/// </summary>
		/// <param name="performanceCounter"></param>
		public static void TestRunSinglePerformanceCounter(PerformanceCounter performanceCounter)
		{
			// Bind the PerformanceCounter to a manager
			var manager = new CounterManager(performanceCounter);
			string value = null;
			var values = new List<string>();

			performanceCounter.NextValue();
			var count = 0;
			while (count < 10)
			{
				value = manager.GetCounterValue();
				Console.WriteLine(performanceCounter.NextValue());
				values.Add(value);
				Thread.Sleep(1000);
				count++;
			}
		}

		/// <summary>
		/// Prints to the console the CounterSample of the counter in it's current state
		/// </summary>
		/// <param name="s"></param>
		public static void OutputSample(CounterSample s)
		{
			Console.WriteLine("\r\n+++++++++++");
			Console.WriteLine("Sample values - \r\n");
			Console.WriteLine("   BaseValue        = " + s.BaseValue);
			Console.WriteLine("   CounterFrequency = " + s.CounterFrequency);
			Console.WriteLine("   CounterTimeStamp = " + s.CounterTimeStamp);
			Console.WriteLine("   CounterType      = " + s.CounterType);
			Console.WriteLine("   RawValue         = " + s.RawValue);
			Console.WriteLine("   SystemFrequency  = " + s.SystemFrequency);
			Console.WriteLine("   TimeStamp        = " + s.TimeStamp);
			Console.WriteLine("   TimeStamp100nSec = " + s.TimeStamp100nSec);
			Console.WriteLine("++++++++++++++++++++++");
		}

		/// <summary>
		/// Builds new GUID value. Used to uniquely identify counters in it's session
		/// </summary>
		/// <returns>new GUID value</returns>
		public static Guid GetNewGuid()
		{
			return Guid.NewGuid();
		}

		#endregion
	}
}