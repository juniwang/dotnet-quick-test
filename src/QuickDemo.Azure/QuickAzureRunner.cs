﻿using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Azure.Management.ContainerService.Fluent;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System;
using Microsoft.Azure.Management.Network.Fluent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Management.Network.Fluent.Models;
using Microsoft.Rest.Azure;

namespace QuickDemo.Azure
{
    public class QuickAzureRunner
    {
        private static QuickAzureRunner runner = new QuickAzureRunner();
        private IAzure _azure = null;
        private SubscriptionConfig _config = null;
        private QuickAzureRunner() { }
        public static QuickAzureRunner Instance
        {
            get
            {
                return runner;
            }
        }

        private IAzure Azure
        {
            get
            {
                if (_azure == null)
                {
                    _config = SubscriptionConfigLoader.LoadFromFile();
                    var subscription = new AzureSubscriptionARM(_config);
                    return subscription.GetAzure();
                }
                return _azure;
            }
        }

        #region VM
        public void VMUsageTest()
        {
            Action<IEnumerable<IComputeUsage>> printUsage = (usages) =>
            {
                foreach (var u in usages)
                {
                    Console.WriteLine($"{u.Name.Value}: {u.CurrentValue}/{u.Limit} {u.Unit.Value}");
                }
            };

            var usage1 = Azure.VirtualMachines.Manager.Usages.ListByRegion(Region.USEast);
            printUsage(usage1);
            Console.WriteLine(usage1.FirstOrDefault(p => p.Name.Value.Equals("cores", StringComparison.OrdinalIgnoreCase))?.Name.Value);

            Console.WriteLine();
            Console.WriteLine();
            usage1 = Azure.VirtualMachines.Manager.Usages.ListByRegion(Region.USWest);
            printUsage(usage1);
        }

        public void VMSize()
        {
            var sizes = Azure.VirtualMachines.Sizes.ListByRegion("eastus");
            foreach (var size in sizes)
            {
                Console.WriteLine(size.ToString());
                Console.WriteLine($"name:{size.Name}, cores:{size.NumberOfCores}");
            }
        }
        #endregion

        #region App Service
        public void ListAllWebApps()
        {
            var webapps = Azure.WebApps.List();
            foreach (var webapp in webapps)
            {
                var config = Azure.WebApps.Inner
                    .GetConfigurationWithHttpMessagesAsync(webapp.ResourceGroupName, webapp.Name)
                    .GetAwaiter().GetResult().Body;

                Console.WriteLine(config);
            }
        }

        public void GetAndConfigWebApp()
        {
            var webappId = "/subscriptions/1c5b82ee-9294-4568-b0c0-b9c523bc0d86/resourceGroups/jw-webapp-win-01/providers/Microsoft.Web/sites/jw-webapp-win-01";
            var webapp = Azure.WebApps.GetById(webappId);
            Console.WriteLine(webapp.ClientCertEnabled);
            webapp.Update()
                .WithClientCertEnabled(true)
                .Apply();

            webapp = Azure.WebApps.GetById(webappId);
            Console.WriteLine(webapp.ClientCertEnabled);
        }
        #endregion

        #region ACS
        public async void ACSCreateAndUpdateTest()
        {
            var lbs = Azure.LoadBalancers.ListByResourceGroup("jsacsdeva");
            foreach (var item in lbs)
            {
                Console.WriteLine(item.Name);
            }

            var acs = await Azure.ContainerServices.GetByIdAsync("/subscriptions/9caf2a1e-9c49-49b6-89a2-56bdec7e3f97/resourceGroups/jwlocalacsa/providers/Microsoft.ContainerService/containerServices/jwlocalacsa");
            if (acs == null)
            {
                acs = await Azure.ContainerServices.Define("jw-acs-001")
                   .WithRegion(Region.USEast)
                   .WithNewResourceGroup("jw-acs-rg-001")
                   .WithKubernetesOrchestration()
                   .WithServicePrincipal(_config.ClientId, _config.ClientSecret)
                   .WithLinux()
                   .WithRootUsername("azureuser")
                   .WithSshKey("ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAACAQDMOwpyk8AbS7cpG1YnG205RTfu5/IcoW8ZsCwUpXPEVRdJaSNQqHjLq5pe1Ep2gJ8HxDoFl6fA5qyQKZ8hmL8pnvHjfR0qA9CoD1LAibngE3JlmyxNwgIZ2Xicsr3JH0J4s5ur0f4wmkjwp5eb9IZgRjLWHfysSt9+RAHGP7ibHRf3N1K0tL8+sqto5JxxhghbLbljVJ+FwERZy9+SxGmyLrQyKgRmYgNqyrkxNb+JvFC26Cmw39sVrJkt42Ifpit7n9Ta85/V0dYFrMtk2mLMoGgKAZGFTRfF/Euth63EZcPyHbMypY4X/OWCNMg6xv8c/JhCz23kjJgEys+UIJGXjo3xxBy+5NwyWs6caNwXdxi//ylmJYjyrTDJsIjSvtxJ62V6NFDdFi0NU+jC/tazbksgsg0WWndMNLguKkH/6Hwox/1SwZRhbyw0rufh5aSoR22QA+e4/MlyWGSqdi7cnJHTwEb2WJhB7lISrChpueQqUUa8RwJsTyCTBfrNs1NfW26EO1Bcqe+K1qldT2sj0hz3gOoQeowP0BeDR02/IOQYHFktsdzRDTzK6OyTHQr5D/mvffVoSB+hS5zCJhRhDDc0yjojKBjvfA7WY902b1hqGKMeY5eABHWiRCUm4eHnTkgRXBHB8cSFzqQxwOT1kmL3B0CJmeD8zNYEo1ltdw== juniwang@microsoft.com")
                   .WithMasterNodeCount(Microsoft.Azure.Management.ContainerService.Fluent.ContainerServiceMasterProfileCount.MIN)
                   .DefineAgentPool("jwdefaultpool")
                   .WithVirtualMachineCount(1)
                   .WithVirtualMachineSize(ContainerServiceVirtualMachineSizeTypes.StandardDS2)
                   .WithDnsPrefix("jwacsagenta")
                   .Attach()
                   .WithMasterDnsPrefix("jwacsmastera")
                   .CreateAsync();
            }
            else
            {
                //await acs.Update().WithAgentVMCount(3).ApplyAsync();
                Console.WriteLine(acs.AgentPools.First().Value.Count);
                var lb = Azure.LoadBalancers.GetByResourceGroup(acs.ResourceGroupName, acs.MasterDnsPrefix);
                if (lb != null)
                {
                    foreach (var ip in lb.Frontends)
                    {
                        Console.WriteLine($"\t{ip.Key}: {ip.Value.Key},{ip.Value.Name} - {ip.ToString()}");
                    }
                }
            }
        }
        #endregion

        #region Network
        class NetworkUsageIn : Wrapper<UsageInner>, INetworkUsage
        {
            public NetworkUsageUnit Unit => ExpandableStringEnum<NetworkUsageUnit>.Parse(UsageInner.Unit);

            public long Limit => Inner.Limit;

            public UsageName Name => Inner.Name;

            public long CurrentValue => Inner.CurrentValue;

            public NetworkUsageIn(UsageInner inner) : base(inner)
            {

            }
        }

        public void NetworkQuota()
        {
            var Ops = Azure.Networks.Manager.Inner.Usages;
            var region = "eastus";

            var first = Extensions.Synchronize(() => Ops.ListAsync(region));
            var result = Extensions.AsContinuousCollection(
                first,
                (string link) => Extensions.Synchronize(() => Ops.ListNextAsync(link)));

            var usages = result.Select(p => new NetworkUsageIn(p)).AsEnumerable< INetworkUsage>();

            foreach (var u in result)
            {

                Console.WriteLine($"{u.Name.Value}: {u.CurrentValue}/{u.Limit}");
            }
        }
        #endregion

        public void ResourceTest()
        {
            var dnsRe = ResourceId.FromString("/subscriptions/9caf2a1e-9c49-49b6-89a2-56bdec7e3f97/resourceGroups/jwsignalrdeva/providers/Microsoft.Network/dnszones/servicedev.signalr.net");
            Console.WriteLine(dnsRe.Name);
        }

        public void RegionTest()
        {
            Region region = Region.Create("East US");
            Console.WriteLine(region.ToString());

            var region2 = Region.Create("westus");
            Console.WriteLine(region2.ToString());
        }
    }
}
