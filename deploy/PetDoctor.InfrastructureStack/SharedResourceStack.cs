﻿using Pulumi;
using Pulumi.Azure.Authorization;
using Pulumi.Azure.ContainerService;
using Pulumi.Azure.ContainerService.Inputs;
using Pulumi.Azure.Core;
using Pulumi.Azure.Network;
using Pulumi.AzureAD;
using System;
using Pulumi.Azure.OperationalInsights;
using Pulumi.Azure.OperationalInsights.Inputs;
using Pulumi.Random;
using Pulumi.Tls;
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
            const string passwordExpiryDate = "2025-01-01T00:00:00Z";
            const int nodeCount = 2;

            var password = new RandomPassword("aks-app-sp-password", new RandomPasswordArgs
            {
                Length = 20,
                Special = true,
            });

            var sshPublicKey = new PrivateKey("ssh-key", new PrivateKeyArgs
            {
                Algorithm = "RSA",
                RsaBits = 4096,
            });

            var config = new Pulumi.Config();
            var kubernetesVersion = config.Get("kubernetesVersion") ?? "1.16.10";
            var environment = config.Get("environment") ?? "development";
            var createdBy = config.Get("createdBy") ?? "default";

            var tags = new InputMap<string>
            {
                { "Environment", environment },
                { "CreatedBy", createdBy }
            };

            var resourceGroup = new ResourceGroup("rg", new ResourceGroupArgs
            {
                Name = $"{prefix}rg",
                Location = "East US 2",
                Tags = tags
            });

            // Create a virtual network and subnet for the AKS cluster
            var vnet = new VirtualNetwork("vnet", new VirtualNetworkArgs
            {
                Name = $"{prefix}vnet",
                Location = resourceGroup.Location,
                ResourceGroupName = resourceGroup.Name,
                AddressSpaces = { "10.5.0.0/16" },
                Tags = tags
            });

            var subnet = new Subnet("subnet", new SubnetArgs
            {
                Name = $"{prefix}subnet",
                ResourceGroupName = resourceGroup.Name,
                AddressPrefixes = { "10.5.1.0/24" },
                VirtualNetworkName = vnet.Name,
                
            });

            // Setup the AD Service Principal for the AKS cluster
            var adApp = new Application("aks-app", new ApplicationArgs { Name = $"{prefix}aksapp" });
            var adSp = new ServicePrincipal("aks-app-sp", new ServicePrincipalArgs { ApplicationId = adApp.ApplicationId });
            var adSpPassword = new ServicePrincipalPassword("aks-app-sp-pwd", new ServicePrincipalPasswordArgs
            {
                ServicePrincipalId = adSp.ObjectId,
                EndDate = DateTime.Parse(passwordExpiryDate).ToString("O"),
                Value = password.Result
            });

            // Grant networking permissions to the SP (needed e.g. to provision Load Balancers)
            var subnetAssignment = new Assignment("subnet-assignment", new AssignmentArgs
            {
                PrincipalId = adSp.Id,
                RoleDefinitionName = "Network Contributor",
                Scope = subnet.Id
            });

            // Setup container registry and allow the AKS SP to pull from it
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
                PrincipalId = adSp.Id,
                RoleDefinitionName = "AcrPull",
                Scope = registry.Id
            });

            // Setup log analytics for the AKS cluster
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
                SolutionName = "ContainerInsights",
                WorkspaceName = logAnalyticsWorkspace.Name,
                WorkspaceResourceId = logAnalyticsWorkspace.Id,
                Plan = new AnalyticsSolutionPlanArgs
                {
                    Product = "OMSGallery/ContainerInsights",
                    Publisher = "Microsoft"
                }
            });

            // Provision the AKS cluster
            var cluster = new KubernetesCluster("aks", new KubernetesClusterArgs
            {
                Name = $"{prefix}aks",
                ResourceGroupName = resourceGroup.Name,
                Location = resourceGroup.Location,
                DnsPrefix = "dns",
                KubernetesVersion = kubernetesVersion,
                DefaultNodePool = new KubernetesClusterDefaultNodePoolArgs
                {
                    Name = "aksagentpool",
                    NodeCount = nodeCount,
                    VmSize = "Standard_D2_v2",
                    OsDiskSizeGb = 30,
                    VnetSubnetId = subnet.Id
                },
                LinuxProfile = new KubernetesClusterLinuxProfileArgs
                {
                    AdminUsername = "aksuser",
                    SshKey = new KubernetesClusterLinuxProfileSshKeyArgs
                    {
                        KeyData = sshPublicKey.PublicKeyOpenssh
                    }
                },
                ServicePrincipal = new KubernetesClusterServicePrincipalArgs
                {
                    ClientId = adSp.ApplicationId,
                    ClientSecret = adSpPassword.Value
                },
                RoleBasedAccessControl = new KubernetesClusterRoleBasedAccessControlArgs
                {
                    Enabled = true
                },
                NetworkProfile = new KubernetesClusterNetworkProfileArgs
                {
                    NetworkPlugin = "azure",
                    ServiceCidr = "10.5.2.0/24",
                    DnsServiceIp = "10.5.2.254",
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
            });
        }
    }
}
