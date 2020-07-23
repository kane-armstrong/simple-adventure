using Pulumi;
using Pulumi.Azure.Authorization;
using Pulumi.Azure.ContainerService;
using Pulumi.Azure.ContainerService.Inputs;
using Pulumi.Azure.Core;
using Pulumi.Azure.Network;
using Pulumi.AzureAD;
using System;
using Pulumi.Azure.OperationalInsights;
using Pulumi.Azure.OperationalInsights.Inputs;
using Application = Pulumi.AzureAD.Application;
using ApplicationArgs = Pulumi.AzureAD.ApplicationArgs;
using VirtualNetwork = Pulumi.Azure.Network.VirtualNetwork;
using VirtualNetworkArgs = Pulumi.Azure.Network.VirtualNetworkArgs;

namespace PetDoctor.InfrastructureStack
{
    public class SharedResourceStack : Stack
    {
        public SharedResourceStack()
        {
            const string prefix = "petdoctor";
            const string password = "";
            const int nodeCount = 2;
            const string sshKey = "";

            var tags = new InputMap<string>
            {
                { "Environment", "development" },
                { "CreatedBy", "Kane Armstrong" }
            };

            var resourceGroup = new ResourceGroup("rg", new ResourceGroupArgs
            {
                Name = $"{prefix}rg",
                Location = "East US 2",
                Tags = tags
            });

            // Setup Service Principal for AKS

            var app = new Application("aks-app", new ApplicationArgs
            {
                Name = $"{prefix}aksapp"
            });

            var sp = new ServicePrincipal("aks-app-sp", new ServicePrincipalArgs
            {
                ApplicationId = app.ApplicationId
            }, new CustomResourceOptions
            {
                DependsOn = new InputList<Resource> { app }
            });

            var sppwd = new ServicePrincipalPassword("aks-app-sp-pwd", new ServicePrincipalPasswordArgs
            {
                ServicePrincipalId = sp.ObjectId,
                EndDate = DateTime.UtcNow.AddYears(3).ToString("O"),
                Value = password
            });

            // Setup network and assignment contributor rights to the AKS SP

            var vnet = new VirtualNetwork("vnet", new VirtualNetworkArgs
            {
                Name = $"{prefix}vnet",
                Location = resourceGroup.Location,
                ResourceGroupName = resourceGroup.Name,
                AddressSpaces = { "10.0.0.0/16" },
                Tags = tags
            });

            var subnet = new Subnet("subnet", new SubnetArgs
            {
                Name = $"{prefix}subnet",
                ResourceGroupName = resourceGroup.Name,
                AddressPrefixes = { "10.0.0.0/24" },
                VirtualNetworkName = vnet.Name
            });

            var subnetAssignment = new Assignment("subnet-assignment", new AssignmentArgs
            {
                PrincipalId = sp.Id,
                RoleDefinitionName = "Network Contributor",
                Scope = subnet.Id
            });

            // Setup container registry and allow the AKS SP pull permissions

            var registry = new Registry("acr", new RegistryArgs
            {
                Name = $"{prefix}acr",
                ResourceGroupName = resourceGroup.Name,
                Location = resourceGroup.Location,
                Sku = "Standard",
                AdminEnabled = false,
                Tags = tags
            });

            var acrAssignment = new Assignment("acr-assignment", new AssignmentArgs
            {
                PrincipalId = sp.Id,
                RoleDefinitionName = "AcrPull",
                Scope = registry.Id
            });

            // Setup log analytics for containers in AKS to use

            var logAnalyticsWorkspace = new AnalyticsWorkspace("log-analytics", new AnalyticsWorkspaceArgs
            {
                Name = "petdoctorloganalyticsworkspace",
                Sku = "PerGB2018",
                Location = resourceGroup.Location,
                ResourceGroupName = resourceGroup.Name,
                Tags = tags
            });

            var logAnalyticsSolution = new AnalyticsSolution("analytics-solution", new AnalyticsSolutionArgs
            {
                ResourceGroupName = resourceGroup.Name,
                Location = resourceGroup.Location,
                SolutionName = "petdoctorcontainerinsights",
                WorkspaceName = logAnalyticsWorkspace.Name,
                WorkspaceResourceId = logAnalyticsWorkspace.Id,
                Plan = new AnalyticsSolutionPlanArgs
                {
                    Product = "OMSGallery/ContainerInsights",
                    Publisher = "Microsoft"
                }
            });

            // Create the AKS cluster

            var aks = new KubernetesCluster("aks", new KubernetesClusterArgs
            {
                Name = $"{prefix}aks",
                ResourceGroupName = resourceGroup.Name,
                Location = resourceGroup.Location,
                DnsPrefix = "dns",
                DefaultNodePool = new KubernetesClusterDefaultNodePoolArgs
                {
                    Name = "default",
                    NodeCount = nodeCount,
                    VmSize = "Standard_D2_v2",
                    OsDiskSizeGb = 30
                },
                LinuxProfile = new KubernetesClusterLinuxProfileArgs
                {
                    AdminUsername = "azureuser",
                    SshKey = new KubernetesClusterLinuxProfileSshKeyArgs
                    {
                        KeyData = sshKey
                    }
                },
                ServicePrincipal = new KubernetesClusterServicePrincipalArgs
                {
                    ClientId = sp.ApplicationId,
                    ClientSecret = sppwd.Value
                },
                RoleBasedAccessControl = new KubernetesClusterRoleBasedAccessControlArgs
                {
                    Enabled = true
                },
                NetworkProfile = new KubernetesClusterNetworkProfileArgs
                {
                    NetworkPlugin = "azure",
                    ServiceCidr = "10.10.0.0/16",
                    DnsServiceIp = "10.10.0.10",
                    DockerBridgeCidr = "172.17.0.1/16"
                },
                AddonProfile = new KubernetesClusterAddonProfileArgs
                {
                    OmsAgent = new KubernetesClusterAddonProfileOmsAgentArgs
                    {
                        Enabled = true,
                        LogAnalyticsWorkspaceId = logAnalyticsWorkspace.Id
                    }
                },
                Tags = tags
            },
            new CustomResourceOptions
            {
                DependsOn = new InputList<Resource> { acrAssignment, subnetAssignment }
            });
        }
    }
}
