using Pulumi;
using Pulumi.Azure.Authorization;
using Pulumi.Azure.ContainerService;
using Pulumi.Azure.ContainerService.Inputs;
using Pulumi.Azure.Core;
using Pulumi.Azure.Network;
using Pulumi.Azure.OperationalInsights;
using Pulumi.Azure.OperationalInsights.Inputs;
using Pulumi.Azure.Sql;
using Pulumi.AzureAD;
using Pulumi.Random;
using Pulumi.Tls;
using System;
using Pulumi.Azure.AppInsights;
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
            var config = new Pulumi.Config();
            var kubernetesVersion = config.Get("kubernetesVersion") ?? "1.16.10";
            var kubernetesNodeCount = config.GetInt32("kubernetesNodeCount") ?? 2;
            var prefix = config.Get("prefix") ?? "petdoctor";
            var environmentCode = config.Get("environmentCode");
            var adSpPasswordExpiryDate = config.Get("spPasswordExpiresOn") ?? "2025-01-01T00:00:00Z";

            var environment = config.Get("environment") ?? "development";
            var createdBy = config.Get("createdBy") ?? "default";
            var sqlUser = config.Get("sqlAdmin") ?? "petdoctoradmin";
            var sqlPassword = config.RequireSecret("sqlPassword");

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
            var vnet = new VirtualNetwork($"{prefix}vnet", new VirtualNetworkArgs
            {
                Location = resourceGroup.Location,
                ResourceGroupName = resourceGroup.Name,
                AddressSpaces = { "10.0.0.0/8" },
                Tags = tags
            });

            var subnet = new Subnet($"{prefix}subnet", new SubnetArgs
            {
                ResourceGroupName = resourceGroup.Name,
                AddressPrefixes = { "10.240.0.0/16" },
                VirtualNetworkName = vnet.Name,

            });

            // Setup the AD Service Principal for the AKS cluster
            var adApp = new Application("aks-app", new ApplicationArgs { Name = $"{prefix}aksapp" });
            var adSp = new ServicePrincipal("aks-app-sp", new ServicePrincipalArgs { ApplicationId = adApp.ApplicationId });
            var adSpPassword = new ServicePrincipalPassword("aks-app-sp-pwd", new ServicePrincipalPasswordArgs
            {
                ServicePrincipalId = adSp.ObjectId,
                EndDate = DateTime.Parse(adSpPasswordExpiryDate).ToString("O"),
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
                    NodeCount = kubernetesNodeCount,
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
                    ServiceCidr = "10.2.0.0/24",
                    DnsServiceIp = "10.2.0.10",
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

            // Create a SQL Server instance
            var sqlServer = new SqlServer($"{prefix}sql", new SqlServerArgs
            {
                Location = resourceGroup.Location,
                ResourceGroupName = resourceGroup.Name,
                Tags = tags,
                Version = "12.0",
                AdministratorLogin = sqlUser,
                AdministratorLoginPassword = sqlPassword
            });

            // Create a SQL database
            var sqlDb = new Database($"{prefix}db", new DatabaseArgs
            {
                Location = resourceGroup.Location,
                ResourceGroupName = resourceGroup.Name,
                ServerName = sqlServer.Name,
                RequestedServiceObjectiveName = "S0",
                Tags = tags
            });

            // Create an Application Insights instance
            var appInsights = new Insights($"{prefix}insights", new InsightsArgs
            {
                ApplicationType = "Web",
                Location = resourceGroup.Location,
                ResourceGroupName = resourceGroup.Name,
                Tags = tags
            });
        }
    }
}
