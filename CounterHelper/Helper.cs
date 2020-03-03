using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CounterHelper
{
	public static class Helper
	{
		public static void GetAllCounters(string categoryFilter)
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

		public static List<CounterData> GetCounterDataForDB()
        {
			List<CounterData> counterDataList = new List<CounterData>();

			foreach (var counter in CounterManagerList.counterManagerList)
			{
				if ((bool)counter.options.SendToDB)
				{
					counterDataList.Add(new CounterData()
					{
						GUID = counter.guid.ToString(),
						Name = counter.options.Name,
						StartTime = counter._startTime,
						LastUpdate = counter._lastUpdate,
						FirstValue = counter._firstValue,
						LastValue = counter._lastValue,
						CategoryName = counter.options.CategoryName,
						CounterName = counter.options.CounterName,
						InstanceName = counter.options.InstanceName,
						MachineName = counter.options.MachineName,
						ReadOnly = counter.options.ReadOnly,
						IterationLength = counter.options.IterationLength,
						Units = counter.options.Units,
						CounterHelp = counter.counterHelp
					});
				}

			}

			return counterDataList;
        }

		public static List<CounterData> GetCounterData()
		{
			return CounterManagerList.counterManagerList.Select(counterManager => new CounterData()
			{
				GUID = counterManager.guid.ToString(),
				Name = counterManager.options.Name,
				StartTime = counterManager._startTime,
				LastUpdate = counterManager._lastUpdate,
				FirstValue = counterManager._firstValue,
				LastValue = counterManager._lastValue,
				CategoryName = counterManager.options.CategoryName,
				CounterName = counterManager.options.CounterName,
				InstanceName = counterManager.options.InstanceName,
				MachineName = counterManager.options.MachineName,
				ReadOnly = counterManager.options.ReadOnly,
				IterationLength = counterManager.options.IterationLength,
				Units = counterManager.options.Units,
				CounterHelp = counterManager.counterHelp
			})
				.ToList();
		}

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

		public static PerformanceCounter GetPerfCounterForProcessId(int processId, string processCounterName = "% Processor Time")
		{
			string instance = GetInstanceNameForProcessId(processId);
			return string.IsNullOrEmpty(instance) ? null : new PerformanceCounter("Process", processCounterName, instance);
		}


		public static void PrintCounter(CounterManager counter)
		{
			string counterLine;

			while (true)
			{
				try
				{
					if (counter.CounterReady() && string.IsNullOrEmpty(counter._options.Name))
					{
						counterLine = counter._options.CategoryName + " \\ " + counter._options.CounterName + " : " +
									  counter.GetCounterValue();
					}
					else
					{
						counterLine = counter._options.Name + ": " + counter._options.CategoryName + " \\ " +
									  counter._options.CounterName + " : " + counter.GetCounterValue();
					}

					Console.WriteLine(counterLine);
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
			}
		}

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

        public static void GetAllCountersForAllCategories(PerformanceCounterCategory[] performanceCounterCategories)
        {
            foreach (var category in performanceCounterCategories)
            {
                GetAllCountersForACategory(category.CategoryName);
            }
        }

		public static void GetAllCountersForACategory(string categoryName)
		{

			//Get all performance categories
			PerformanceCounterCategory[] perfCats = PerformanceCounterCategory.GetCategories();

			//Get single category by category name.
			PerformanceCounterCategory cat = perfCats.FirstOrDefault(c => c.CategoryName == categoryName);
			if (cat != null)
			{
				Console.WriteLine("Category Name: {0}", cat.CategoryName);

				//Get all instances available for category
				string[] instances = cat.GetInstanceNames();
				if (instances.Length == 0)
				{
					//This block will execute when category has no instance.
					//loop all the counters available within the category
					foreach (PerformanceCounter counter in cat.GetCounters())
						Console.WriteLine("     Counter Name: {0}", counter.CounterName);
				}
				else
				{
					//This block will execute when category has one or more instances.
					foreach (string instance in instances)
					{
						Console.WriteLine("  Instance Name: {0}", instance);
						if (cat.InstanceExists(instance))
							//loop all the counters available within the category
							foreach (PerformanceCounter counter in cat.GetCounters(instance))
								Console.WriteLine("     Counter Name: {0}", counter.CounterName);
					}
				}
			}
		}

		public static void TestRunSinglePerformanceCounter(PerformanceCounter performanceCounter)
		{
			performanceCounter.NextValue();
			var count = 0;
			while (count < 10)
			{
				Console.WriteLine(performanceCounter.NextValue());
				Thread.Sleep(1000);
				count++;
			}
		}

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

		public static Guid GetNewGuid()
		{
			return Guid.NewGuid();
		}
	}
}