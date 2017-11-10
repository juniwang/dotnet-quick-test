using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickDemo.Azure
{
    public class AzureSubscriptionARM
    {
        SubscriptionConfig _subscriptionConfig;
        string _authority;

        public AzureSubscriptionARM(SubscriptionConfig config)
        {
            _subscriptionConfig = config;
            _authority = config.Environment().AuthenticationEndpoint + config.TenantId ?? "common" + "/";
        }

        private AzureCredentials GetCredential()
        {
            var credentials = SdkContext.AzureCredentialsFactory
                       .FromServicePrincipal(_subscriptionConfig.ClientId,
                       _subscriptionConfig.ClientSecret,
                       _subscriptionConfig.TenantId,
                       AzureEnvironment.AzureGlobalCloud);

            return credentials;
        }


        public IAzure GetAzure()
        {
            var azure = Microsoft.Azure.Management.Fluent.Azure
                .Configure()
                .Authenticate(GetCredential())
                .WithSubscription(_subscriptionConfig.SubscriptionId);

            return azure;
        }
    }
}
