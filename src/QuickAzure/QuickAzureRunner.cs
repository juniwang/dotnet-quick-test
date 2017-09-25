﻿using Microsoft.Azure.Management.Fluent;
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
                    var subscription = new AnotherSubscription(_config);
                    return subscription.GetAzure();
                }
                return _azure;
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

        public async void ACSCreateAndUpdateTest()
        {
            var acs = await Azure.ContainerServices.GetByIdAsync("/subscriptions/2085065b-00f8-4cba-9675-ba15f4d4ab66/resourceGroups/jw-acs-rg-001/providers/Microsoft.ContainerService/containerServices/jw-acs-001");
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
                   .WithMasterNodeCount(Microsoft.Azure.Management.Compute.Fluent.ContainerServiceMasterProfileCount.MIN)
                   .WithMasterLeafDomainLabel("jwacsmastera")
                   .DefineAgentPool("jwdefaultpool")
                   .WithVMCount(1)
                   .WithVMSize("Standard_DS2")
                   .WithLeafDomainLabel("jwacsagenta")
                   .Attach()
                   .CreateAsync();
            }
            else
            {
                await acs.Update().WithAgentVMCount(3)
                    .ApplyAsync();
            }

            foreach (var item in acs.Manager.ResourceManager.Deployments.ListByResourceGroup("jw-acs-rg-001"))
            {
                Console.WriteLine(item.Name + "    " + item.Timestamp.ToString());
            }

        }
    }
}
