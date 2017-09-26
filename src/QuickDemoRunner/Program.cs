using QuickAzure;
using QuickDemo.Common.String;
using QuickDemo.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickDemo
{
    class Foo
    {
        public string Name { get; set; }

        public Foo(string name)
        {
            Name = name;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            //QuickAzureRunner.Instance.CoresTest();
            QuickDemoStorageRunner.Instance.SumTest();
            //Console.WriteLine(StringUtils.Aggregate());
            //Console.WriteLine(StringUtils.Aggregate("Test"));
            //Console.WriteLine(StringUtils.Aggregate("this", "is", "a", "test"));
            Console.Read();
        }
    }
}
