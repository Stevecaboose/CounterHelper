using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CounterHelper
{
    public class CounterData
    {
        public string GUID { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime LastUpdate { get; set; }
        public string FirstValue { get; set; }
        public string LastValue { get; set; }
        public string CategoryName { get; set; }
        public string CounterName { get; set; }
        public string InstanceName { get; set; }
        public string MachineName { get; set; }
        public bool? ReadOnly { get; set; }
        public int? IterationLength { get; set; }
		public string Units { get; set; }
		public string CounterHelp { get; set; }

    }
}
