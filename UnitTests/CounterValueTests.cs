using System;
using System.Linq;
using System.Threading;
using CounterHelper;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class CounterValueTests
    {
        private Options CPU_Processor_Time_Counter;
        private Options System_Up_Time;

        [SetUp]
        public void SetUp()
        {
            CPU_Processor_Time_Counter = new Options()
            {
                Name = "CPU Percent",
                CategoryName = "Processor",
                CounterName = "% Processor Time",
                InstanceName = "_Total",
                MachineName = ".",
                ReadOnly = true,
                IterationLength = 1000
            };

            System_Up_Time = new Options()
            {
                Name = "System Up Time",
                CategoryName = "System",
                CounterName = "System Up Time",
                MachineName = ".",
                ReadOnly = true,
                IterationLength = 1000
            };
        }

        [Test]
        public void GetProcessorTime()
        {

            ICounterManager manager = new CounterManager(CPU_Processor_Time_Counter);

            //Act
            var firstValue = float.Parse(manager.GetCounterValue());
            Thread.Sleep(1000);
            var secondValue = float.Parse(manager.GetCounterValue());

            // Assert
            Assert.IsTrue(firstValue > 0 && secondValue > 0);

        }

        [Test]
        public void GetSystemUpTime()
        {
            ICounterManager manager = new CounterManager(System_Up_Time);

            var firstValue = TimeSpan.Parse(manager.GetCounterValue());
            Thread.Sleep(1000);
            var secondValue = TimeSpan.Parse(manager.GetCounterValue());

            Assert.Less(firstValue, secondValue);
        }

        [Test]
        public void StopCounter()
        {

            ICounterManager manager = new CounterManager(CPU_Processor_Time_Counter);

            bool startState;
            bool endState;

            //start counter on its own thread
            var thread = new Thread(() => manager.StartCounter());
            try
            {
                
                thread.Start();
                startState = manager.GetStartStopState();
                manager.StopCounter();
                endState = manager.GetStartStopState();

                Assert.IsFalse(startState);
                Assert.IsTrue(endState);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            thread.Abort();



        }

    }
}
