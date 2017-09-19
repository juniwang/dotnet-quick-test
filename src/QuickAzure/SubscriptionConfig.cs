using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickAzure
{
    public class SubscriptionConfig
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string SubscriptionId { get; set; }
        public CloudEnvironment CloudEnvironment { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string TenantId { get; set; }
    }

    public enum CloudEnvironment
    {
        Azure,
        AzureChina,
        AzureUSGovernment,
        AzureGermany,
    }

    public static class SubscripionConfigExtensions
    {
        public static AzureEnv Environment(this SubscriptionConfig config)
        {
            switch (config.CloudEnvironment)
            {
                case CloudEnvironment.AzureChina:
                    return AzureEnv.AzureChina;
                case CloudEnvironment.AzureUSGovernment:
                    return AzureEnv.AzureUSGovernment;
                case CloudEnvironment.AzureGermany:
                    return AzureEnv.AzureGermany;
                default:
                    return AzureEnv.Azure;
            }
        }
    }
}
