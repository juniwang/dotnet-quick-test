using QuickDemo.Azure;
using QuickDemo.Common.String;
using QuickDemo.Storage;
using QuickDemo.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickDemo.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            QuickWindowsRunner.Instance.PerformanceCounterTest();
            //QuickWindowsRunner.Instance.LogEventTest();
            //QuickAzureRunner.Instance.ListAllWebApps();
            //QuickDemoStorageRunner.Instance.SumTest();
            //Console.WriteLine(StringUtils.Aggregate());
            //Console.WriteLine(StringUtils.Aggregate("Test"));
            //Console.WriteLine(StringUtils.Aggregate("this", "is", "a", "test"));
            Console.Read();
        }
    }
}
