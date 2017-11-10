using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuickDemo.Azure
{
    public class QuickAzureRunnerClassic
    {
        private static QuickAzureRunnerClassic runner = new QuickAzureRunnerClassic();
        private QuickAzureRunnerClassic() { }
        public static QuickAzureRunnerClassic Instance
        {
            get
            {
                return runner;
            }
        }

        public void RunTokenAcquire()
        {
            var config = SubscriptionConfigLoader.LoadFromFile();
            var subscription = new AzureSubscriptionClassic(config);

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
