using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CounterHelper
{
    public class PerformanceCounterCategoryObject
    {
        public PerformanceCounterCategory Category { get; set; }
        public List<string> Instance { get; set; }
        
    }
}
