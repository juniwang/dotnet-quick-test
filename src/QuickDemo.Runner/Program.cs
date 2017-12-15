using QuickDemo.Azure;
using QuickDemo.Common.String;
using QuickDemo.Framework;
using QuickDemo.Storage;
using QuickDemo.Windows;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace QuickDemo.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            //QuickFrameworkRunner.Instance.MultiCount();
            //QuickWindowsRunner.Instance.PerformanceCounterTest();
            //QuickWindowsRunner.Instance.LogEventTest();
            //QuickAzureRunner.Instance.VMUsageTest();
            //QuickDemoStorageRunner.Instance.SumTest();
            //Console.WriteLine(StringUtils.Aggregate());
            //Console.WriteLine(StringUtils.Aggregate("Test"));
            //Console.WriteLine(StringUtils.Aggregate("this", "is", "a", "test"));
            KVTest();

            Console.Read();
        }

        static async void KVTest()
        {
            var kv = new KeyVaultTester("1056e7fe-a55d-44f3-8173-370c26358c94", "C35CBFF9FA6C51E51E1DE97B6D1E246F27661301", "https://jwkvlocala.vault.azure.net");
            var name = "TestingSecret";
            var secret = await kv.GetSecretAsync(name);
            Console.WriteLine(secret);

            await kv.SetSecretAsync(name, "newValue");
            secret = await kv.GetSecretAsync(name);
            Console.WriteLine(secret);
        }
    }
}
