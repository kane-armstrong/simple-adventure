using PetDoctor.InfrastructureStack.Core;
using Pulumi;
using Pulumi.Azure.AppInsights;
using Pulumi.Azure.Authorization;
using Pulumi.Azure.ContainerService;
using Pulumi.Azure.ContainerService.Inputs;
using Pulumi.Azure.Core;
using Pulumi.Azure.KeyVault;
using Pulumi.Azure.KeyVault.Inputs;
using Pulumi.Azure.Network;
using Pulumi.Azure.OperationalInsights;
using Pulumi.Azure.OperationalInsights.Inputs;
using Pulumi.Azure.Sql;
using Pulumi.AzureAD;
using Pulumi.Docker;
using Pulumi.Kubernetes.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Meta.V1;
using Pulumi.Kubernetes.Types.Inputs.Networking.V1Beta1;
using Pulumi.Kubernetes.Yaml;
using Pulumi.Random;
using Pulumi.Tls;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Text;
using Application = Pulumi.AzureAD.Application;
using ApplicationArgs = Pulumi.AzureAD.ApplicationArgs;
using ContainerArgs = Pulumi.Kubernetes.Types.Inputs.Core.V1.ContainerArgs;
using CustomResource = Pulumi.Kubernetes.ApiExtensions.CustomResource;
using Deployment = Pulumi.Kubernetes.Apps.V1.Deployment;
using DeploymentArgs = Pulumi.Kubernetes.Types.Inputs.Apps.V1.DeploymentArgs;
using DeploymentSpecArgs = Pulumi.Kubernetes.Types.Inputs.Apps.V1.DeploymentSpecArgs;
using GetClientConfig = Pulumi.AzureAD.GetClientConfig;
using Ingress = Pulumi.Kubernetes.Networking.V1Beta1.Ingress;
using Provider = Pulumi.Kubernetes.Provider;
using Secret = Pulumi.Kubernetes.Core.V1.Secret;
using SecretArgs = Pulumi.Kubernetes.Types.Inputs.Core.V1.SecretArgs;
using Service = Pulumi.Kubernetes.Core.V1.Service;
using ServiceArgs = Pulumi.Kubernetes.Types.Inputs.Core.V1.ServiceArgs;
using VirtualNetwork = Pulumi.Azure.Network.VirtualNetwork;
using VirtualNetworkArgs = Pulumi.Azure.Network.VirtualNetworkArgs;

namespace PetDoctor.InfrastructureStack
{
    // TODO: There are a bunch of things that I don't like about this class that I want to fix:
    // - [Output] props - we need to review these. Are any missing? Any not necessary? Does the naming make sense?
    // - The ctor is huge, unwieldy, and unmaintainable. My first cut at mitigating this was regions, but that sucks. Need an alternative
    // - Configuration needs to be revised. Some variables may be droppable, location should be passed in (e.g. eastus2), and we should use the azure:region notation for keys
    // - Resources declarations are not very cohesive. Grouping would probably be better done by deployable than resource type

    [SuppressMessage("ReSharper", "UnusedVariable")]
    public class PetDoctorStack : Stack
    {
        [Output] public Output<string> ContainerRegistryLoginServer { get; set; }

        [Output] public Output<string> KubeConfig { get; set; }

        [Output] public Output<string> AppInsightsInstrumentationKey { get; set; }

        [Output] public Output<string> KeyVaultUri { get; set; }

        [Output] public Output<string> AppointmentApiIdentityResourceId { get; set; }

        [Output] public Output<string> AppointmentApiIdentityClientId { get; set; }

        [Output] public Output<string> AppointmentApiIdentityObjectId { get; set; }

        public PetDoctorStack()
        {
            #region Configuration

            var config = new Pulumi.Config();

            var kubernetesVersion = config.Get("kubernetesVersion") ?? "1.16.10";
            var kubernetesNodeCount = config.GetInt32("kubernetesNodeCount") ?? 2;

            var kubeNamespace = config.Get("chartNamespace") ?? "pet-doctor-dev";

            var prefix = config.Get("prefix") ?? "petdoctor";

            var environment = config.Get("environment") ?? "development";
            var createdBy = config.Get("createdBy") ?? "default";
            var sqlUser = config.Get("sqlAdmin") ?? "petdoctoradmin";
            var sqlPassword = config.RequireSecret("sqlPassword");

            var tenantId = config.RequireSecret("tenantId");

            var tags = new InputMap<string>
            {
                { "Environment", environment },
                { "CreatedBy", createdBy }
            };

            #endregion

            #region Resource group

            var resourceGroup = new ResourceGroup("rg", new ResourceGroupArgs
            {
                Name = $"{prefix}rg",
                Location = "East US 2",
                Tags = tags
            });

            #endregion

            #region Network setup

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
                ServiceEndpoints = new InputList<string> { "Microsoft.KeyVault", "Microsoft.Sql" }
            });

            #endregion

            #region Container registry setup

            var registry = new Registry("acr", new RegistryArgs
            {
                Name = $"{prefix}acr",
                ResourceGroupName = resourceGroup.Name,
                Location = resourceGroup.Location,
                Sku = "Standard",
                AdminEnabled = true,
                Tags = tags
            });

            ContainerRegistryLoginServer = registry.LoginServer;

            #endregion

            #region Cluster service principal setup

            var password = new RandomPassword("aks-app-sp-password", new RandomPasswordArgs
            {
                Length = 20,
                Special = true,
            });

            var adApp = new Application("aks-app", new ApplicationArgs { Name = $"{prefix}aksapp" });
            var adSp = new ServicePrincipal("aks-app-sp", new ServicePrincipalArgs { ApplicationId = adApp.ApplicationId });
            var adSpPassword = new ServicePrincipalPassword("aks-app-sp-pwd", new ServicePrincipalPasswordArgs
            {
                ServicePrincipalId = adSp.ObjectId,
                EndDate = "2099-01-01T00:00:00Z",
                Value = password.Result
            });

            // Grant networking permissions to the SP (needed e.g. to provision Load Balancers)
            var subnetAssignment = new Assignment("subnet-assignment", new AssignmentArgs
            {
                PrincipalId = adSp.Id,
                RoleDefinitionName = "Network Contributor",
                Scope = subnet.Id
            });

            var acrAssignment = new Assignment("acr-assignment", new AssignmentArgs
            {
                PrincipalId = adSp.Id,
                RoleDefinitionName = "AcrPull",
                Scope = registry.Id
            });

            #endregion

            #region Cluster logging setup

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

            #endregion

            #region Kubernetes cluster setup

            var sshPublicKey = new PrivateKey("ssh-key", new PrivateKeyArgs
            {
                Algorithm = "RSA",
                RsaBits = 4096,
            });

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
                    ClientId = adApp.ApplicationId,
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

            KubeConfig = cluster.KubeConfigRaw;

            var provider = new Provider("k8s", new Pulumi.Kubernetes.ProviderArgs
            {
                KubeConfig = cluster.KubeConfigRaw
            });

            #endregion

            #region SQL Server instance/database(s) setup

            var sqlServer = new SqlServer($"{prefix}sql", new SqlServerArgs
            {
                Location = resourceGroup.Location,
                ResourceGroupName = resourceGroup.Name,
                Tags = tags,
                Version = "12.0",
                AdministratorLogin = sqlUser,
                AdministratorLoginPassword = sqlPassword
            });

            var sqlvnetrule = new VirtualNetworkRule($"sqlvnetrule", new VirtualNetworkRuleArgs
            {
                ResourceGroupName = resourceGroup.Name,
                ServerName = sqlServer.Name,
                SubnetId = subnet.Id,
            });

            var sqlDb = new Database($"{prefix}db", new DatabaseArgs
            {
                Location = resourceGroup.Location,
                ResourceGroupName = resourceGroup.Name,
                ServerName = sqlServer.Name,
                RequestedServiceObjectiveName = "S0",
                Tags = tags
            });

            #endregion

            #region Application Insights setup

            // Create an Application Insights instance
            var sharedAppInsights = new Insights($"{prefix}-ai", new InsightsArgs
            {
                ApplicationType = "web",
                Location = resourceGroup.Location,
                ResourceGroupName = resourceGroup.Name,
                Tags = tags
            });

            AppInsightsInstrumentationKey = sharedAppInsights.InstrumentationKey;

            #endregion

            #region Managed identities setup

            var appointmentApiIdentity = new UserAssignedIdentity("appointment-api", new UserAssignedIdentityArgs
            {
                ResourceGroupName = resourceGroup.Name,
                Location = resourceGroup.Location,
                Tags = tags
            });

            AppointmentApiIdentityResourceId = appointmentApiIdentity.Id;

            AppointmentApiIdentityClientId = appointmentApiIdentity.ClientId;

            AppointmentApiIdentityObjectId = appointmentApiIdentity.PrincipalId;

            // Assigning the AKS SP the correct role membership (Managed Identity Operator) for the user assigned identities is mandatory

            var aksSpAppointmentApiAccessPolicy = new Assignment("aks-sp-appontment-api", new AssignmentArgs
            {
                PrincipalId = adSp.ObjectId,
                RoleDefinitionName = "Managed Identity Operator",
                Scope = appointmentApiIdentity.Id
            });

            var sqlAdmin = new ActiveDirectoryAdministrator("sqladmin", new ActiveDirectoryAdministratorArgs
            {
                ResourceGroupName = resourceGroup.Name,
                TenantId = tenantId,
                ObjectId = appointmentApiIdentity.PrincipalId,
                Login = "sqladmin",
                ServerName = sqlServer.Name
            });

            #endregion

            #region KeyVault setup

            var clientConfig = Output.Create(GetClientConfig.InvokeAsync());
            var currentPrincipalTenantId = clientConfig.Apply(c => c.TenantId);
            var currentPrincipal = clientConfig.Apply(c => c.ObjectId);

            // Create a KeyVault instance
            var appointmentApiKeyVault = new KeyVault($"{prefix}kv", new KeyVaultArgs
            {
                Location = resourceGroup.Location,
                ResourceGroupName = resourceGroup.Name,
                EnabledForDiskEncryption = true,
                TenantId = tenantId,
                SkuName = "standard",
                AccessPolicies = new InputList<KeyVaultAccessPolicyArgs>
                {
                    new KeyVaultAccessPolicyArgs
                    {
                        TenantId = tenantId,
                        ObjectId = adSp.ObjectId,
                        SecretPermissions = new[] {"get", "list"}
                    },
                    new KeyVaultAccessPolicyArgs
                    {
                        TenantId = tenantId,
                        ObjectId = appointmentApiIdentity.PrincipalId,
                        SecretPermissions = new[] {"get", "list"}
                    },
                    new KeyVaultAccessPolicyArgs
                    {
                        TenantId = currentPrincipalTenantId,
                        ObjectId = currentPrincipal,
                        SecretPermissions = {"delete", "get", "list", "set"},
                    }
                },
                NetworkAcls = new KeyVaultNetworkAclsArgs
                {
                    DefaultAction = "Deny",
                    Bypass = "AzureServices",
                    VirtualNetworkSubnetIds = new InputList<string>
                    {
                        subnet.Id
                    },
                    IpRules = new InputList<string>
                    {
                        GetMyPublicIpAddress()
                    }
                },
                Tags = tags
            });

            KeyVaultUri = appointmentApiKeyVault.VaultUri;

            var secret = new Pulumi.Azure.KeyVault.Secret("appointment-db-connection-string", new Pulumi.Azure.KeyVault.SecretArgs
            {
                KeyVaultId = appointmentApiKeyVault.Id,
                Name = "ConnectionStrings--PetDoctorContext",
                Value = Output.Tuple(sqlServer.Name, sqlServer.Name, sqlServer.AdministratorLogin, sqlServer.AdministratorLoginPassword).Apply(t =>
                {
                    var (server, database, administratorLogin, administratorLoginPassword) = t;
                    return $"Server=tcp:{server}.database.windows.net;Database={database};User ID={administratorLogin};Password={administratorLoginPassword}";
                })
            }, new CustomResourceOptions
            {
                DependsOn = sqlServer
            });

            #endregion

            #region Cluster setup

            var aadPodIdentityDeployment = new ConfigFile("aad-pod-identity", new ConfigFileArgs
            {
                File = "https://raw.githubusercontent.com/Azure/aad-pod-identity/master/deploy/infra/deployment-rbac.yaml"
            }, new ComponentResourceOptions
            {
                DependsOn = cluster,
                Provider = provider
            });

            var certManagerDeployment = new ConfigFile("cert-manager", new ConfigFileArgs
            {
                File = "https://raw.githubusercontent.com/jetstack/cert-manager/release-0.8/deploy/manifests/00-crds.yaml"
            }, new ComponentResourceOptions
            {
                DependsOn = cluster,
                Provider = provider
            });

            var nginxDeployment = new ConfigFile("nginx", new ConfigFileArgs
            {
                File = "https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v0.34.1/deploy/static/provider/cloud/deploy.yaml"
            }, new ComponentResourceOptions
            {
                DependsOn = cluster,
                Provider = provider
            });

            var clusterNamespace = new Namespace(kubeNamespace, new NamespaceArgs
            {
                Metadata = new ObjectMetaArgs
                {
                    Name = kubeNamespace
                }
            }, new CustomResourceOptions
            {
                DependsOn = cluster,
                Provider = provider
            });

            var clusterIssuer = new CustomResource("cert-manager-cluster-issuer", new CertManagerClusterIssuerResourceArgs
            {
                Metadata = new ObjectMetaArgs
                {
                    Name = "letsencrypt-prod",
                    Namespace = kubeNamespace
                },
                Spec = new CertManagerClusterIssuerSpecArgs
                {
                    Acme = new CertManagerClusterIssuerAcmeArgs
                    {
                        Email = "kane.armstrong@outlook.com",
                        Server = "https://acme-v02.api.letsencrypt.org/directory",
                        PrivateKeySecretRef = new CertManagerClusterIssuerAcmeSecretArgs
                        {
                            Name = "letsencrypt-prod"
                        }
                    }
                }
            }, new CustomResourceOptions
            {
                DependsOn = cluster,
                Provider = provider
            });

            #endregion

            var values = new PetDoctorValues
            {
                Name = "pet-doctor",
                Host = "petdoctor.kanearmstrong.com",
                Namespace = kubeNamespace,
                SecretName = "pet-doctor-secrets",
                AppointmentApi = new ReplicaSetConfiguration
                {
                    AadPodIdentityBindingName = "appointment-api-pod-identity-binding",
                    AadPodIdentityName = "appointment-api-pod-identity",
                    AadPodIdentitySelector = "appointment-api",
                    DeploymentName = "pet-doctor-appointment-api",
                    IngressName = "pet-doctor-appointment-api-ingress",
                    ServiceName = "pet-doctor-appointment-api-svc",
                    Image = registry.LoginServer.Apply(loginServer => $"{loginServer}/pet-doctor/appointments/api:{config.Require("appVersion")}"),
                    Port = 80,
                    ReplicaCount = 2,
                    Cpu = new ResourceLimit
                    {
                        Request = "25m",
                        Limit = "50m"
                    },
                    Memory = new ResourceLimit
                    {
                        Request = "250Mi",
                        Limit = "400Mi"
                    }
                }
            };

            var secrets = new Secret(values.SecretName, new SecretArgs
            {
                Metadata = new ObjectMetaArgs
                {
                    Namespace = kubeNamespace,
                    Name = values.SecretName
                },
                Kind = "Secret",
                ApiVersion = "v1",
                Type = "Opaque",
                Data = new InputMap<string>
                {
                    { "keyvault-url", appointmentApiKeyVault.VaultUri.Apply(kvUrl => Convert.ToBase64String(Encoding.UTF8.GetBytes(kvUrl))) },
                    { "appinsights-instrumentationkey", sharedAppInsights.InstrumentationKey.Apply(key => Convert.ToBase64String(Encoding.UTF8.GetBytes(key))) }
                }
            }, new CustomResourceOptions
            {
                DependsOn = cluster,
                Provider = provider
            });

            var image = new Image("appointment-api-docker", new ImageArgs
            {
                Build = $".{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}",
                Registry = new ImageRegistry
                {
                    Server = registry.LoginServer,
                    Username = registry.AdminUsername,
                    Password = registry.AdminPassword
                },
                ImageName = values.AppointmentApi.Image
            }, new ComponentResourceOptions
            {
                DependsOn = new InputList<Resource> { cluster, registry },
                Provider = provider
            });

            SetupAppointmentApiInKubernetes(values, appointmentApiIdentity, cluster, provider);
        }

        private static void SetupAppointmentApiInKubernetes(PetDoctorValues values,
            UserAssignedIdentity appointmentApiIdentity, KubernetesCluster cluster, ProviderResource provider)
        {
            var appointmentApiPodIdentity = new CustomResource(values.AppointmentApi.AadPodIdentityName,
                new AzureIdentityResourceArgs
                {
                    Metadata = new ObjectMetaArgs
                    {
                        Name = values.AppointmentApi.AadPodIdentityName
                    },
                    Spec = new AzureIdentitySpecArgs
                    {
                        Type = 0,
                        ResourceId = appointmentApiIdentity.Id,
                        ClientId = appointmentApiIdentity.ClientId
                    }
                }, new CustomResourceOptions
                {
                    DependsOn = cluster,
                    Provider = provider
                });

            var appointmentApiPodIdentityBinding = new CustomResource(values.AppointmentApi.AadPodIdentityBindingName,
                new AzureIdentityBindingResourceArgs
                {
                    Metadata = new ObjectMetaArgs
                    {
                        Name = values.AppointmentApi.AadPodIdentityBindingName
                    },
                    Spec = new AzureIdentityBindingSpecArgs
                    {
                        AzureIdentity = values.AppointmentApi.AadPodIdentityName,
                        Selector = values.AppointmentApi.AadPodIdentitySelector
                    }
                }, new CustomResourceOptions
                {
                    DependsOn = new InputList<Resource> { cluster, appointmentApiIdentity },
                    Provider = provider
                });

            var appointmentApiDeployment = new Deployment("appointment-api-deployment", new DeploymentArgs
            {
                ApiVersion = "apps/v1beta1",
                Kind = "Deployment",
                Metadata = new ObjectMetaArgs
                {
                    Name = values.AppointmentApi.DeploymentName,
                    Namespace = values.Namespace,
                    Labels = new InputMap<string>
                    {
                        {"app", values.AppointmentApi.DeploymentName},
                        {"aadpodidbinding", values.AppointmentApi.AadPodIdentitySelector}
                    }
                },
                Spec = new DeploymentSpecArgs
                {
                    Replicas = values.AppointmentApi.ReplicaCount,
                    Selector = new LabelSelectorArgs
                    {
                        MatchLabels = new InputMap<string>
                        {
                            {"app", values.AppointmentApi.DeploymentName}
                        }
                    },
                    Template = new PodTemplateSpecArgs
                    {
                        Metadata = new ObjectMetaArgs
                        {
                            Labels = new InputMap<string>
                            {
                                {"app", values.AppointmentApi.DeploymentName},
                                {"aadpodidbinding", values.AppointmentApi.AadPodIdentitySelector}
                            }
                        },
                        Spec = new PodSpecArgs
                        {
                            Containers = new InputList<ContainerArgs>
                            {
                                new ContainerArgs
                                {
                                    Name = values.AppointmentApi.DeploymentName,
                                    Image = values.AppointmentApi.Image,
                                    Ports = new InputList<ContainerPortArgs>
                                    {
                                        new ContainerPortArgs
                                        {
                                            ContainerPortValue = values.AppointmentApi.Port,
                                            Protocol = "TCP"
                                        }
                                    },
                                    ReadinessProbe = new ProbeArgs
                                    {
                                        HttpGet = new HTTPGetActionArgs
                                        {
                                            Port = values.AppointmentApi.Port,
                                            Path = "/ready",
                                            Scheme = "HTTP"
                                        },
                                        InitialDelaySeconds = 15,
                                        TimeoutSeconds = 2,
                                        PeriodSeconds = 10,
                                        FailureThreshold = 3
                                    },
                                    LivenessProbe = new ProbeArgs
                                    {
                                        HttpGet = new HTTPGetActionArgs
                                        {
                                            Port = values.AppointmentApi.Port,
                                            Path = "/live",
                                            Scheme = "HTTP"
                                        },
                                        InitialDelaySeconds = 30,
                                        TimeoutSeconds = 2,
                                        PeriodSeconds = 5,
                                        FailureThreshold = 3
                                    },
                                    Env = new InputList<EnvVarArgs>
                                    {
                                        new EnvVarArgs
                                        {
                                            Name = "ASPNETCORE_ENVIRONMENT",
                                            Value = "Production"
                                        },
                                        new EnvVarArgs
                                        {
                                            Name = "KEYVAULT__URL",
                                            ValueFrom = new EnvVarSourceArgs
                                            {
                                                SecretKeyRef = new SecretKeySelectorArgs
                                                {
                                                    Name = values.SecretName,
                                                    Key = "keyvault-url"
                                                }
                                            }
                                        },
                                        new EnvVarArgs
                                        {
                                            Name = "APPLICATIONINSIGHTS__INSTRUMENTATIONKEY",
                                            ValueFrom = new EnvVarSourceArgs
                                            {
                                                SecretKeyRef = new SecretKeySelectorArgs
                                                {
                                                    Name = values.SecretName,
                                                    Key = "appinsights-instrumentationkey"
                                                }
                                            }
                                        },
                                        new EnvVarArgs
                                        {
                                            Name = "HOSTENV",
                                            Value = "K8S"
                                        },
                                        new EnvVarArgs
                                        {
                                            Name = "PATH_BASE",
                                            Value = "api"
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }, new CustomResourceOptions
            {
                DependsOn = new InputList<Resource> { cluster, appointmentApiIdentity, appointmentApiPodIdentityBinding },
                Provider = provider
            });

            var appointmentApiService = new Service(values.AppointmentApi.ServiceName, new ServiceArgs
            {
                ApiVersion = "v1",
                Kind = "Service",
                Metadata = new ObjectMetaArgs
                {
                    Name = values.AppointmentApi.ServiceName,
                    Namespace = values.Namespace
                },
                Spec = new ServiceSpecArgs
                {
                    Selector = new InputMap<string>
                    {
                        {"app", values.AppointmentApi.DeploymentName}
                    },
                    Type = "ClusterIP",
                    ClusterIP = "None",
                    Ports = new InputList<ServicePortArgs>
                    {
                        new ServicePortArgs
                        {
                            Name = "http",
                            Protocol = "TCP",
                            Port = 80,
                            TargetPort = values.AppointmentApi.Port
                        }
                    }
                }
            }, new CustomResourceOptions
            {
                DependsOn = cluster,
                Provider = provider
            });

            var appointmentApiIngress = new Ingress(values.AppointmentApi.IngressName, new IngressArgs
            {
                ApiVersion = "extensions/v1beta1",
                Kind = "Ingress",
                Metadata = new ObjectMetaArgs
                {
                    Name = values.AppointmentApi.IngressName,
                    Namespace = values.Namespace,
                    Annotations = new InputMap<string>
                    {
                        {"kubernetes.io/ingress.class", "nginx"},
                        {"certmanager.k8s.io/cluster-issuer", "letsencrypt-prod"}
                    }
                },
                Spec = new IngressSpecArgs
                {
                    Tls = new InputList<IngressTLSArgs>
                    {
                        new IngressTLSArgs
                        {
                            Hosts = new InputList<string>
                            {
                                values.Host
                            },
                            SecretName = "tls-secret"
                        }
                    },
                    Rules = new InputList<IngressRuleArgs>
                    {
                        new IngressRuleArgs
                        {
                            Host = values.Host,
                            Http = new HTTPIngressRuleValueArgs
                            {
                                Paths = new InputList<HTTPIngressPathArgs>
                                {
                                    new HTTPIngressPathArgs
                                    {
                                        Path = "/api",
                                        Backend = new IngressBackendArgs
                                        {
                                            ServiceName = values.AppointmentApi.ServiceName,
                                            ServicePort = values.AppointmentApi.Port
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }, new CustomResourceOptions
            {
                DependsOn = new InputList<Resource>
                {
                    cluster, appointmentApiDeployment, appointmentApiService
                },
                Provider = provider
            });
        }

        public static string GetMyPublicIpAddress()
        {
            using var wc = new WebClient();
            return wc.DownloadString("http://icanhazip.com");
        }
    }
}
