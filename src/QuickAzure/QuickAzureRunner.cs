using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuickAzure
{
    public class QuickAzureRunner
    {
        public static void RunTokenAcquire()
        {
            var config = SubscriptionConfigLoader.LoadFromFile();
            var subscription = new AzureSubscription(config);

            using (var client = subscription.CreateManagementClient())
            {
                var resp = client.Subscriptions.GetAsync(CancellationToken.None).Result;
                Console.WriteLine("SubscriptionName=" + resp.SubscriptionName);
                Console.WriteLine("SubscriptionID=" + resp.SubscriptionID);
                Console.WriteLine("Subscription=" + resp.ToString());
            }
        }

        public static void GetAndConfigWebApp()
        {
            var config = SubscriptionConfigLoader.LoadFromFile();
            var subscription = new AnotherSubscription(config);
            var azure = subscription.GetAzure();

            var webappId = "/subscriptions/1c5b82ee-9294-4568-b0c0-b9c523bc0d86/resourceGroups/jw-webapp-win-01/providers/Microsoft.Web/sites/jw-webapp-win-01";
            var webapp = azure.WebApps.GetById(webappId);
            Console.WriteLine(webapp.ClientCertEnabled);
            webapp.Update()
                .WithClientCertEnabled(true)
                .Apply();

            webapp = azure.WebApps.GetById(webappId);
            Console.WriteLine(webapp.ClientCertEnabled);
        }
    }
}
