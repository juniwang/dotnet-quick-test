using QuickDemo.Azure;
using QuickDemo.Common;
using QuickDemo.Common.Certificate;
using QuickDemo.Common.String;
using QuickDemo.Framework;
using QuickDemo.Storage;
using QuickDemo.Windows;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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
            //QuickAzureRunner.Instance.ACSCreateAndUpdateTest();
            //QuickDemoStorageRunner.Instance.InsertOrUpdate();
            //QuickCommonRunner.Instance.GetWithCertificate("https://signalrdev.eastus.cloudapp.azure.com:5000/api/values", "C35CBFF9FA6C51E51E1DE97B6D1E246F27661301");
            QuickCommonRunner.Instance.AutofacAdapter();

            //Console.WriteLine(StringUtils.Aggregate());
            //Console.WriteLine(StringUtils.Aggregate("Test"));
            //Console.WriteLine(StringUtils.Aggregate("this", "is", "a", "test"));
            //KVTest();

            Console.Read();
        }

        static async void KVTest()
        {
            var kv = new KeyVaultTester("624c0e2f-6122-4b26-a229-06431f82e6b3", "C35CBFF9FA6C51E51E1DE97B6D1E246F27661301", "https://kvsignalrdeva.vault.azure.net");
            var name = "kube-config-srdevacsrpb";
            var secret = await kv.GetSecretAsync(name);
            Console.WriteLine(secret);

            //await kv.SetSecretAsync(name, "newValue");
            //secret = await kv.GetSecretAsync(name);
            //Console.WriteLine(secret);
        }
    }
}
