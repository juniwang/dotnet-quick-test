using QuickAzure;
using QuickDemo.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //QuickAzureRunner.Instance.ACSCreateAndUpdateTest();
            QuickDemoStorageRunner.Instance.TableCaseQuery();
            Console.Read();
        }
    }
}
