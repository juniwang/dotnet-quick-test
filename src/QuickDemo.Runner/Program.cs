using Microsoft.WindowsAzure.Wapd.Acis.SME.ConfigurationSchema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuickDemo.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            //QuickFrameworkRunner.Instance.MultiCount();
            //QuickFrameworkRunner.Instance.HeartBeat();
            //QuickWindowsRunner.Instance.PerformanceCounterTest();
            //QuickWindowsRunner.Instance.LogEventTest();
            QuickAzureRunner.Instance.VMSize();
            //QuickDemoStorageRunner.Instance.SASTest();
            //QuickDemoStorageRunner.Instance.QueueMessageVisibleTest();
            //QuickCommonRunner.Instance.GetWithCertificate("https://localhost:5000/api/values", "C35CBFF9FA6C51E51E1DE97B6D1E246F27661301");
            //QuickCommonRunner.Instance.ObjectSort();

            //Console.WriteLine(StringUtils.Aggregate());
            //Console.WriteLine(StringUtils.Aggregate("Test"));
            //Console.WriteLine(StringUtils.Aggregate("this", "is", "a", "test"));

            //KVTest();
            //TempTest();

            Console.Read();
        }

        static void TempTest()
        {
            var t = new TempTest
            {
                Parameter = new TempParameter { ApiVersion = "1.0", SubId = "000-000" },
                Responses = new Dictionary<string, TempResponse> { },
            };
            var json = JsonConvert.SerializeObject(t);
            Console.WriteLine(json);

            object o = JsonConvert.DeserializeObject<object>(json);
            Console.WriteLine(JsonConvert.SerializeObject(o));
        }

        static async void KVTest()
        {
            var kv = new KeyVaultTester("624c0e2f-6122-4b26-a229-06431f82e6b3", "C35CBFF9FA6C51E51E1DE97B6D1E246F27661301", "https://kvsignalrdeva.vault.azure.net");
            var name = "signalrdf";
            var secret = await kv.GetSecretAsync(name);
            Console.WriteLine(secret);

            var bytes = Convert.FromBase64String(secret);

            var coll = new X509Certificate2Collection();
            coll.Import(bytes, null, X509KeyStorageFlags.Exportable);
            X509Certificate2 certificate = coll[0];
            Console.WriteLine(certificate.FriendlyName);

            //await kv.SetSecretAsync(name, "vvv");
            //secret = await kv.GetSecretAsync(name);
            //Console.WriteLine(secret);
        }
    }

    public class Cert
    {
        public string data { get; set; }
        public string password { get; set; }
    }

    public class TempTest
    {
        [JsonProperty("parameters")]
        public TempParameter Parameter { get; set; }

        [JsonProperty("responses")]
        public Dictionary<string, TempResponse> Responses { get; set; }
    }

    public class TempParameter
    {
        [JsonProperty("api-version")]
        public string ApiVersion { get; set; }

        [JsonProperty("subscriptionId")]
        public string SubId { get; set; }
    }

    public class TempResponse
    {
        public string Id { get; set; }
        public int Count { get; set; }
    }
}
