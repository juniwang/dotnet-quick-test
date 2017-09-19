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

        public static void RunTokenAcquireFluent()
        {
            var config = SubscriptionConfigLoader.LoadFromFile();
            var subscription = new AnotherSubscription(config);
            var azure = subscription.GetAzure();
            var sub = subscription.GetAzure().GetCurrentSubscription();
            //var usage = subscription.GetAzure().VirtualMachines.Manager.Usages.ListByRegion(Region.USEast).ToList();

            Console.WriteLine("SubscriptionName=" + sub.DisplayName);
            Console.WriteLine("SubscriptionId=" + sub.SubscriptionId);
            Console.WriteLine("Subscription=" + sub.ToString());
        }
    }
}
