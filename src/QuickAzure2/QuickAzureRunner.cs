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
    }
}
