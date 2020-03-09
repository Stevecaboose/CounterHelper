using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CounterHelper
{
    public class Options
    {
        #region Public Properties
        
        public int Id { get; set; }

        public string Name { get; set; }

        public string CategoryName { get; set; }

        public string CounterName { get; set; }

        public string InstanceName { get; set; }

        public string MachineName { get; set; }

        public bool? ReadOnly { get; set; }

        public int? IterationLength { get; set; }

		public string Units { get; set; }

        public Guid GuidValue => _guidValue.Equals(Guid.Empty) ? Helper.GetNewGuid() : _guidValue;

        #endregion

        #region Private Variables

        private Guid _guidValue;

        #endregion

        /// <summary>
        /// Prints an easy to read list of properties for the calling Option
        /// </summary>
        /// <returns></returns>
        public string PrintOptions()
        {
            var isReadOnly = (ReadOnly is null) ? "True" : ReadOnly.Value.ToString();
            var instance = string.IsNullOrEmpty(InstanceName) ? "------" : InstanceName;

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("------------------------");
            stringBuilder.AppendLine("Name: " + Name);
            stringBuilder.AppendLine("GUID: " + GuidValue);
            stringBuilder.AppendLine("Category Name: " + CategoryName);
            stringBuilder.AppendLine("Counter Name: " + CounterName);
            stringBuilder.AppendLine("Instance Name: " + instance);
            stringBuilder.AppendLine("Machine Name: " + MachineName);
            stringBuilder.AppendLine("Is Read-Only: " + isReadOnly);
            stringBuilder.AppendLine("Iteration Length (ms): " + IterationLength);
			stringBuilder.AppendLine("Units: " + Units);
            stringBuilder.AppendLine("------------------------");

            return stringBuilder.ToString();

        }
    }
}