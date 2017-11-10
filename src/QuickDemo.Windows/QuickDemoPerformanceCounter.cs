using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickDemo.Windows
{
    public class QuickDemoPerformanceCounter
    {
        private static readonly string CategoryName = "JunboWang";
        private static readonly string CounterName = "TestCounter";
        private static readonly string CategoryHelp = "A category for custom performance counters";

        private static readonly int SampleRateInMillis = 1000;
        private static readonly int NumberofSamples = 1000000000;
        private static PerformanceCounter perfCounter;
        private static Random random = new Random();

        static QuickDemoPerformanceCounter()
        {
            SetupCategory();
            CreateCounters();
        }

        private static void SetupCategory()
        {
            if (!PerformanceCounterCategory.Exists(CategoryName))
            {
                CounterCreationDataCollection counterCreationDataCollection = new CounterCreationDataCollection();
                CounterCreationData totalBytesReceived = new CounterCreationData();
                totalBytesReceived.CounterType = PerformanceCounterType.NumberOfItems64;
                totalBytesReceived.CounterName = CounterName;
                counterCreationDataCollection.Add(totalBytesReceived);
                PerformanceCounterCategory.Create(CategoryName, CategoryHelp, PerformanceCounterCategoryType.MultiInstance, counterCreationDataCollection);
            }
            else
                Console.WriteLine("Category {0} exists", CategoryName);
        }

        private static void CreateCounters()
        {
            perfCounter = new PerformanceCounter(CategoryName, CounterName, "localhost", false);
            perfCounter.RawValue = 0;
        }

        public static void UpdatePerfCounters()
        {
            for (int i = 0; i < NumberofSamples; i++)
            {
                int r = random.Next(1000, 10000);
                perfCounter.RawValue = r;
                Console.WriteLine("perfCounter.RawValue = {0}", perfCounter.RawValue);
                System.Threading.Thread.Sleep(SampleRateInMillis);
            }
        }
    }
}
