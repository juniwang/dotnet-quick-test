using Microsoft.Azure;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.WindowsAzure.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickAzure
{
    public class AzureSubscription
    {
        SubscriptionConfig _subscriptionConfig;
        AzureEnvironment _azureEnvironment;
        string _authority;

        public AzureSubscription(SubscriptionConfig subscripionConfig)
        {
            _subscriptionConfig = subscripionConfig;
            _azureEnvironment = subscripionConfig.Environment();
            _authority = _azureEnvironment.ActiveDirectoryEndpoint + subscripionConfig.TenantId ?? "common" + "/";
        }

        public override string ToString()
        {
            return _subscriptionConfig.SubscriptionId;
        }

        public string SubscriptionId
        {
            get { return _subscriptionConfig.SubscriptionId; }
        }

        public SubscriptionCloudCredentials GetCredentials(AzureResource resource)
        {
            if (resource == null)
            {
                string message = string.Format("cannot create azure credential due for sub {0} to invalid resource",
                    _subscriptionConfig.SubscriptionId);
                throw new Exception(message);
            }

            var authenticationResult = AcquireAccessTokenAysnc(resource);
            return new TokenCloudCredentials(authenticationResult.AccessToken);
        }

        public AzureEnvironment AzureEnvironment
        {
            get { return _azureEnvironment; }
        }

        private AuthenticationResult AcquireAccessTokenAysnc(AzureResource resource)
        {
            AuthenticationContext authContext = new AuthenticationContext(_authority, false);
            var credentials = new ClientCredential(_subscriptionConfig.ClientId, _subscriptionConfig.ClientSecret);
            return authContext.AcquireTokenAsync(resource.Resource, credentials).Result;
        }
    }

    public static class IAzureSubscriptionExtensions
    {
        public static ManagementClient CreateManagementClient(this AzureSubscription sub)
        {
            return new ManagementClient(
                sub.GetCredentials(AzureResource.ServiceManagement),
                sub.AzureEnvironment.ManagementEndpointUri);
        }
    }
}
