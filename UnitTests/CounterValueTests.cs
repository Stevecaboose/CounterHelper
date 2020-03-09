using System;
using System.Threading;
using CounterHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class CounterValueTests
    {
        [TestMethod]
        public void GetProcessorTime()
        {
            //Arrange
            var option = new Options()
            {
                Name = "CPU Percent",
                CategoryName = "Processor",
                CounterName = "% Processor Time",
                InstanceName = "_Total",
                MachineName = ".",
                ReadOnly = true,
                IterationLength = 1000
            };
            ICounterManager counterHelper = new CounterManager(option);

            //Act
            counterHelper.GetCounterValue();
            Thread.Sleep(1000);
            var firstValue = float.Parse(counterHelper.GetCounterValue());
            Thread.Sleep(1000);
            var secondValue = float.Parse(counterHelper.GetCounterValue());

            CounterManagerList.ForceStopAllCounters();

            // Assert
            Assert.IsTrue(firstValue > 0 && secondValue > 0);

        }

    }
}
