using PetDoctor.InfrastructureStack.Configuration;
using PetDoctor.InfrastructureStack.Kubernetes.AadPodIdentity;
using PetDoctor.InfrastructureStack.Kubernetes.CertManager;
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
using System.IO;
using System.Net;
using System.Text;
using Application = Pulumi.AzureAD.Application;
using ApplicationArgs = Pulumi.AzureAD.ApplicationArgs;
using Config = Pulumi.Config;
using ContainerArgs = Pulumi.Kubernetes.Types.Inputs.Core.V1.ContainerArgs;
using CustomResource = Pulumi.Kubernetes.ApiExtensions.CustomResource;
using Deployment = Pulumi.Kubernetes.Apps.V1.Deployment;
using DeploymentArgs = Pulumi.Kubernetes.Types.Inputs.Apps.V1.DeploymentArgs;
using DeploymentSpecArgs = Pulumi.Kubernetes.Types.Inputs.Apps.V1.DeploymentSpecArgs;
using GetClientConfig = Pulumi.AzureAD.GetClientConfig;
using Ingress = Pulumi.Kubernetes.Networking.V1Beta1.Ingress;
using Provider = Pulumi.Kubernetes.Provider;
using ProviderArgs = Pulumi.Kubernetes.ProviderArgs;
using Secret = Pulumi.Kubernetes.Core.V1.Secret;
using SecretArgs = Pulumi.Kubernetes.Types.Inputs.Core.V1.SecretArgs;
using Service = Pulumi.Kubernetes.Core.V1.Service;
using ServiceArgs = Pulumi.Kubernetes.Types.Inputs.Core.V1.ServiceArgs;
using VirtualNetwork = Pulumi.Azure.Network.VirtualNetwork;
using VirtualNetworkArgs = Pulumi.Azure.Network.VirtualNetworkArgs;
// ReSharper disable UnusedVariable

namespace PetDoctor.InfrastructureStack
{
    public class PetDoctorStack : Stack
    {
        public PetDoctorStack()
        {
            var config = new Config();

            var azureResources = CreateBaseAzureInfrastructure(config);

            var clusterOptions = new PetDoctorClusterOptions
            {
                Domain = config.Require("domain"),
                Namespace = config.Require("kubernetes-namespace"),
                CertificateIssuerAcmeEmail = config.Require("certmanager-acme-email"),
                AppointmentApi = new ReplicaSetConfiguration
                {
                    AadPodIdentityBindingName = "appointments-api-pod-identity-binding",
                    AadPodIdentityName = "appointments-api-pod-identity",
                    AadPodIdentitySelector = "appointments-api",
                    DeploymentName = "appointments-api",
                    IngressName = "appointments-api-ingress",
                    ServiceName = "appointments-api-svc",
                    Image = azureResources.Registry.LoginServer.Apply(loginServer => $"{loginServer}/pet-doctor/appointments/api:{config.Require("versions:appointmentsApi")}"),
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
                    },
                    SecretName = "appointments-api-secrets"
                }
            };

            ConfigureKubernetesCluster(azureResources, clusterOptions);

            var appointmentApiAzureResources = CreateAppointmentApiAzureResources(azureResources, config, clusterOptions.AppointmentApi.Image);

            SetupAppointmentApiInKubernetes(azureResources, appointmentApiAzureResources, clusterOptions);
        }

        private static AzureResourceBag CreateBaseAzureInfrastructure(Config config)
        {
            var location = config.Require("azure-location");

            var environment = config.Require("azure-tags-environment");
            var owner = config.Require("azure-tags-owner");
            var createdBy = config.Require("azure-tags-createdby");

            var kubernetesVersion = config.Require("kubernetes-version");
            var kubernetesNodeCount = config.RequireInt32("kubernetes-scaling-nodecount");

            var sqlUser = config.RequireSecret("azure-sqlserver-username");
            var sqlPassword = config.RequireSecret("azure-sqlserver-password");

            var tags = new InputMap<string>
            {
                { "Environment", environment },
                { "CreatedBy", createdBy },
                { "Owner", owner }
            };

            var resourceGroup = new ResourceGroup("pet-doctor-resource-group", new ResourceGroupArgs
            {
                Name = "pet-doctor",
                Location = location,
                Tags = tags
            });

            var vnet = new VirtualNetwork("pet-doctor-vnet", new VirtualNetworkArgs
            {
                ResourceGroupName = resourceGroup.Name,
                AddressSpaces = { "10.0.0.0/8" },
                Tags = tags
            });

            var subnet = new Subnet("pet-doctor-subnet", new SubnetArgs
            {
                ResourceGroupName = resourceGroup.Name,
                AddressPrefixes = { "10.240.0.0/16" },
                VirtualNetworkName = vnet.Name,
                ServiceEndpoints = new InputList<string> { "Microsoft.KeyVault", "Microsoft.Sql" }
            });

            var registry = new Registry("pet-doctor-acr", new RegistryArgs
            {
                ResourceGroupName = resourceGroup.Name,
                Sku = "Standard",
                AdminEnabled = true,
                Tags = tags
            });

            var aksServicePrincipalPassword = new RandomPassword("pet-doctor-aks-ad-sp-password", new RandomPasswordArgs
            {
                Length = 20,
                Special = true,
            }).Result;

            var clusterAdApp = new Application("pet-doctor-aks-ad-app", new ApplicationArgs
            {
                Name = "pet-doctor-aks"
            });

            var clusterAdServicePrincipal = new ServicePrincipal("aks-app-sp", new ServicePrincipalArgs
            {
                ApplicationId = clusterAdApp.ApplicationId
            });

            var clusterAdServicePrincipalPassword = new ServicePrincipalPassword("aks-app-sp-pwd", new ServicePrincipalPasswordArgs
            {
                ServicePrincipalId = clusterAdServicePrincipal.ObjectId,
                EndDate = "2099-01-01T00:00:00Z",
                Value = aksServicePrincipalPassword
            });

            // Grant networking permissions to the SP (needed e.g. to provision Load Balancers)
            var subnetAssignment = new Assignment("pet-doctor-aks-sp-subnet-assignment", new AssignmentArgs
            {
                PrincipalId = clusterAdServicePrincipal.Id,
                RoleDefinitionName = "Network Contributor",
                Scope = subnet.Id
            });

            var acrAssignment = new Assignment("pet-doctor-aks-sp-acr-assignment", new AssignmentArgs
            {
                PrincipalId = clusterAdServicePrincipal.Id,
                RoleDefinitionName = "AcrPull",
                Scope = registry.Id
            });

            var logAnalyticsWorkspace = new AnalyticsWorkspace("pet-doctor-aks-log-analytics", new AnalyticsWorkspaceArgs
            {
                ResourceGroupName = resourceGroup.Name,
                Name = "petdoctorloganalyticsworkspace",
                Sku = "PerGB2018",
                Tags = tags
            });

            var logAnalyticsSolution = new AnalyticsSolution("pet-doctor-aks-analytics-solution", new AnalyticsSolutionArgs
            {
                ResourceGroupName = resourceGroup.Name,
                SolutionName = "ContainerInsights",
                WorkspaceName = logAnalyticsWorkspace.Name,
                WorkspaceResourceId = logAnalyticsWorkspace.Id,
                Plan = new AnalyticsSolutionPlanArgs
                {
                    Product = "OMSGallery/ContainerInsights",
                    Publisher = "Microsoft"
                }
            });

            var sshPublicKey = new PrivateKey("ssh-key", new PrivateKeyArgs
            {
                Algorithm = "RSA",
                RsaBits = 4096,
            });

            var cluster = new KubernetesCluster("pet-doctor-aks", new KubernetesClusterArgs
            {
                ResourceGroupName = resourceGroup.Name,
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
                    ClientId = clusterAdApp.ApplicationId,
                    ClientSecret = clusterAdServicePrincipalPassword.Value
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

            var sqlServer = new SqlServer("pet-doctor-sql", new SqlServerArgs
            {
                ResourceGroupName = resourceGroup.Name,
                Tags = tags,
                Version = "12.0",
                AdministratorLogin = sqlUser,
                AdministratorLoginPassword = sqlPassword
            });

            var sqlvnetrule = new VirtualNetworkRule("pet-doctor-sql", new VirtualNetworkRuleArgs
            {
                ResourceGroupName = resourceGroup.Name,
                ServerName = sqlServer.Name,
                SubnetId = subnet.Id,
            });

            var appInsights = new Insights("pet-doctor-ai", new InsightsArgs
            {
                ApplicationType = "web",
                ResourceGroupName = resourceGroup.Name,
                Tags = tags
            });

            var provider = new Provider("pet-doctor-aks-provider", new ProviderArgs
            {
                KubeConfig = cluster.KubeConfigRaw
            });

            return new AzureResourceBag
            {
                ResourceGroup = resourceGroup,
                SqlServer = sqlServer,
                Cluster = cluster,
                ClusterProvider = provider,
                AppInsights = appInsights,
                AksServicePrincipal = clusterAdServicePrincipal,
                Subnet = subnet,
                Registry = registry,
                Tags = tags
            };
        }

        private static void ConfigureKubernetesCluster(AzureResourceBag azureResources, PetDoctorClusterOptions clusterOptions)
        {
            var componentOpts = new ComponentResourceOptions
            {
                DependsOn = azureResources.Cluster,
                Provider = azureResources.ClusterProvider
            };

            var customOpts = new CustomResourceOptions
            {
                DependsOn = azureResources.Cluster,
                Provider = azureResources.ClusterProvider
            };

            var aadPodIdentityDeployment = new ConfigFile("k8s-aad-pod-identity", new ConfigFileArgs
            {
                File = "https://raw.githubusercontent.com/Azure/aad-pod-identity/master/deploy/infra/deployment-rbac.yaml"
            }, componentOpts);

            var certManagerDeployment = new ConfigFile("k8s-cert-manager", new ConfigFileArgs
            {
                File = "https://raw.githubusercontent.com/jetstack/cert-manager/release-0.8/deploy/manifests/00-crds.yaml"
            }, componentOpts);

            var nginxDeployment = new ConfigFile("k8s-nginx-ingress", new ConfigFileArgs
            {
                File =
                    "https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v0.34.1/deploy/static/provider/cloud/deploy.yaml"
            }, componentOpts);

            var clusterNamespace = new Namespace("k8s-namespace", new NamespaceArgs
            {
                Metadata = new ObjectMetaArgs
                {
                    Name = clusterOptions.Namespace
                }
            }, customOpts);

            var clusterIssuer = new CustomResource("k8s-cert-manager-cluster-issuer", new CertManagerClusterIssuerResourceArgs
            {
                Metadata = new ObjectMetaArgs
                {
                    Name = "letsencrypt-prod"
                },
                Spec = new CertManagerClusterIssuerSpecArgs
                {
                    Acme = new CertManagerClusterIssuerAcmeArgs
                    {
                        Email = clusterOptions.CertificateIssuerAcmeEmail,
                        Server = "https://acme-v02.api.letsencrypt.org/directory",
                        PrivateKeySecretRef = new CertManagerClusterIssuerAcmeSecretArgs
                        {
                            Name = "letsencrypt-prod"
                        }
                    }
                }
            }, customOpts);

            var certs = new CustomResource("k8s-cert-manager-domain-cert", new CertManagerCertificateResourceArgs
            {
                Metadata = new ObjectMetaArgs
                {
                    Name = "tls-secret"
                },
                Spec = new CertManagerCertificateSpecArgs
                {
                    SecretName = "tls-secret",
                    DnsNames = clusterOptions.Domain,
                    Acme = new CertManagerCertificateAcmeArgs
                    {
                        Config = new CertManagerCertificateAcmeConfigArgs
                        {
                            Http = new CertManagerCertificateAcmeConfigHttpArgs
                            {
                                IngressClass = "nginx"
                            },
                            Domains = clusterOptions.Domain
                        }
                    },
                    IssuerRef = new CertManagerCertificateIssuerRefArgs
                    {
                        Name = "letsencrypt-prod",
                        Kind = "ClusterIssuer"
                    }
                }
            }, customOpts);
        }

        private static AppointmentApiAzureResourceBag CreateAppointmentApiAzureResources(AzureResourceBag azureResources, Config config, Input<string> registryImageName)
        {
            var tenantId = config.Require("azure-tenantId");

            var appointmentApiDb = new Database("appointments-api-db", new DatabaseArgs
            {
                ResourceGroupName = azureResources.ResourceGroup.Name,
                Name = "appointment-api",
                ServerName = azureResources.SqlServer.Name,
                RequestedServiceObjectiveName = "S0",
                Tags = azureResources.Tags
            });

            var image = new Image("appointments-api-docker-image", new ImageArgs
            {
                Build = $".{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}",
                Registry = new ImageRegistry
                {
                    Server = azureResources.Registry.LoginServer,
                    Username = azureResources.Registry.AdminUsername,
                    Password = azureResources.Registry.AdminPassword
                },
                ImageName = registryImageName
            }, new ComponentResourceOptions
            {
                DependsOn = new InputList<Resource> { azureResources.Registry }
            });

            var appointmentApiIdentity = new UserAssignedIdentity("appointments-api", new UserAssignedIdentityArgs
            {
                ResourceGroupName = azureResources.ResourceGroup.Name,
                Tags = azureResources.Tags
            });

            // AKS service principal needs to have Managed Identity Operator rights over the user assigned identity else AAD pod identity won't work
            var aksSpAppointmentApiAccessPolicy = new Assignment("aks-sp-appontment-api-access", new AssignmentArgs
            {
                PrincipalId = azureResources.AksServicePrincipal.ObjectId,
                RoleDefinitionName = "Managed Identity Operator",
                Scope = appointmentApiIdentity.Id
            });

            var sqlAdmin = new ActiveDirectoryAdministrator("appointments-api-sql-access", new ActiveDirectoryAdministratorArgs
            {
                ResourceGroupName = azureResources.ResourceGroup.Name,
                TenantId = tenantId,
                ObjectId = appointmentApiIdentity.PrincipalId,
                Login = "sqladmin",
                ServerName = azureResources.SqlServer.Name
            });

            var clientConfig = Output.Create(GetClientConfig.InvokeAsync());
            var currentPrincipalTenantId = clientConfig.Apply(c => c.TenantId);
            var currentPrincipal = clientConfig.Apply(c => c.ObjectId);

            var appointmentApiKeyVault = new KeyVault("appointment-api-keyvault", new KeyVaultArgs
            {
                ResourceGroupName = azureResources.ResourceGroup.Name,
                Name = "appointment-api",
                EnabledForDiskEncryption = true,
                TenantId = tenantId,
                SkuName = "standard",
                AccessPolicies = new InputList<KeyVaultAccessPolicyArgs>
                {
                    new KeyVaultAccessPolicyArgs
                    {
                        TenantId = tenantId,
                        ObjectId = azureResources.AksServicePrincipal.ObjectId,
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
                        azureResources.Subnet.Id
                    },
                    // Need to whitelist the local public IP address otherwise setting secrets will fail
                    IpRules = new InputList<string>
                    {
                        GetMyPublicIpAddress()
                    }
                },
                Tags = azureResources.Tags
            });

            var secret = new Pulumi.Azure.KeyVault.Secret("appointments-api-db-connection-string",
                new Pulumi.Azure.KeyVault.SecretArgs
                {
                    KeyVaultId = appointmentApiKeyVault.Id,
                    Name = "ConnectionStrings--PetDoctorContext",
                    Value = Output.Tuple(azureResources.SqlServer.Name, azureResources.SqlServer.Name,
                        azureResources.SqlServer.AdministratorLogin, azureResources.SqlServer.AdministratorLoginPassword).Apply(
                        t =>
                        {
                            var (server, database, administratorLogin, administratorLoginPassword) = t;
                            return
                                $"Server=tcp:{server}.database.windows.net;Database={database};User ID={administratorLogin};Password={administratorLoginPassword}";
                        })
                }, new CustomResourceOptions
                {
                    DependsOn = new InputList<Resource>
                    {
                        azureResources.SqlServer,
                        appointmentApiKeyVault
                    }
                });

            return new AppointmentApiAzureResourceBag
            {
                Identity = appointmentApiIdentity,
                KeyVault = appointmentApiKeyVault
            };
        }

        private static void SetupAppointmentApiInKubernetes(AzureResourceBag azureResources, AppointmentApiAzureResourceBag appointmentApiAzureResources, PetDoctorClusterOptions clusterOptions)
        {
            var customOpts = new CustomResourceOptions
            {
                DependsOn = azureResources.Cluster,
                Provider = azureResources.ClusterProvider
            };

            var secret = new Secret(clusterOptions.AppointmentApi.SecretName, new SecretArgs
            {
                Metadata = new ObjectMetaArgs
                {
                    Namespace = clusterOptions.Namespace,
                    Name = clusterOptions.AppointmentApi.SecretName
                },
                Kind = "Secret",
                ApiVersion = "v1",
                Type = "Opaque",
                Data = new InputMap<string>
                {
                    { "keyvault-url", appointmentApiAzureResources.KeyVault.VaultUri.Apply(kvUrl => Convert.ToBase64String(Encoding.UTF8.GetBytes(kvUrl))) },
                    { "appinsights-instrumentationkey", azureResources.AppInsights.InstrumentationKey.Apply(key => Convert.ToBase64String(Encoding.UTF8.GetBytes(key))) }
                }
            }, customOpts);

            var appointmentApiPodIdentity = new CustomResource(clusterOptions.AppointmentApi.AadPodIdentityName,
                new AzureIdentityResourceArgs
                {
                    Metadata = new ObjectMetaArgs
                    {
                        Name = clusterOptions.AppointmentApi.AadPodIdentityName
                    },
                    Spec = new AzureIdentitySpecArgs
                    {
                        Type = 0,
                        ResourceId = appointmentApiAzureResources.Identity.Id,
                        ClientId = appointmentApiAzureResources.Identity.ClientId
                    }
                }, customOpts);

            var appointmentApiPodIdentityBinding = new CustomResource(clusterOptions.AppointmentApi.AadPodIdentityBindingName,
                new AzureIdentityBindingResourceArgs
                {
                    Metadata = new ObjectMetaArgs
                    {
                        Name = clusterOptions.AppointmentApi.AadPodIdentityBindingName
                    },
                    Spec = new AzureIdentityBindingSpecArgs
                    {
                        AzureIdentity = clusterOptions.AppointmentApi.AadPodIdentityName,
                        Selector = clusterOptions.AppointmentApi.AadPodIdentitySelector
                    }
                }, new CustomResourceOptions
                {
                    DependsOn = new InputList<Resource> { azureResources.Cluster, appointmentApiAzureResources.Identity },
                    Provider = azureResources.ClusterProvider
                });

            var appointmentApiDeployment = new Deployment("appointment-api-deployment", new DeploymentArgs
            {
                ApiVersion = "apps/v1beta1",
                Kind = "Deployment",
                Metadata = new ObjectMetaArgs
                {
                    Name = clusterOptions.AppointmentApi.DeploymentName,
                    Namespace = clusterOptions.Namespace,
                    Labels = new InputMap<string>
                    {
                        {"app", clusterOptions.AppointmentApi.DeploymentName},
                        {"aadpodidbinding", clusterOptions.AppointmentApi.AadPodIdentitySelector}
                    }
                },
                Spec = new DeploymentSpecArgs
                {
                    Replicas = clusterOptions.AppointmentApi.ReplicaCount,
                    Selector = new LabelSelectorArgs
                    {
                        MatchLabels = new InputMap<string>
                        {
                            {"app", clusterOptions.AppointmentApi.DeploymentName}
                        }
                    },
                    Template = new PodTemplateSpecArgs
                    {
                        Metadata = new ObjectMetaArgs
                        {
                            Labels = new InputMap<string>
                            {
                                {"app", clusterOptions.AppointmentApi.DeploymentName},
                                {"aadpodidbinding", clusterOptions.AppointmentApi.AadPodIdentitySelector}
                            }
                        },
                        Spec = new PodSpecArgs
                        {
                            Containers = new InputList<ContainerArgs>
                            {
                                new ContainerArgs
                                {
                                    Name = clusterOptions.AppointmentApi.DeploymentName,
                                    Image = clusterOptions.AppointmentApi.Image,
                                    Ports = new InputList<ContainerPortArgs>
                                    {
                                        new ContainerPortArgs
                                        {
                                            ContainerPortValue = clusterOptions.AppointmentApi.Port,
                                            Protocol = "TCP"
                                        }
                                    },
                                    ReadinessProbe = new ProbeArgs
                                    {
                                        HttpGet = new HTTPGetActionArgs
                                        {
                                            Port = clusterOptions.AppointmentApi.Port,
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
                                            Port = clusterOptions.AppointmentApi.Port,
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
                                                    Name = clusterOptions.AppointmentApi.SecretName,
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
                                                    Name = clusterOptions.AppointmentApi.SecretName,
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
                DependsOn = new InputList<Resource> { azureResources.Cluster, appointmentApiAzureResources.Identity, appointmentApiPodIdentityBinding },
                Provider = azureResources.ClusterProvider
            });

            var appointmentApiService = new Service(clusterOptions.AppointmentApi.ServiceName, new ServiceArgs
            {
                ApiVersion = "v1",
                Kind = "Service",
                Metadata = new ObjectMetaArgs
                {
                    Name = clusterOptions.AppointmentApi.ServiceName,
                    Namespace = clusterOptions.Namespace
                },
                Spec = new ServiceSpecArgs
                {
                    Selector = new InputMap<string>
                    {
                        {"app", clusterOptions.AppointmentApi.DeploymentName}
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
                            TargetPort = clusterOptions.AppointmentApi.Port
                        }
                    }
                }
            }, customOpts);

            var appointmentApiIngress = new Ingress(clusterOptions.AppointmentApi.IngressName, new IngressArgs
            {
                ApiVersion = "extensions/v1beta1",
                Kind = "Ingress",
                Metadata = new ObjectMetaArgs
                {
                    Name = clusterOptions.AppointmentApi.IngressName,
                    Namespace = clusterOptions.Namespace,
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
                                clusterOptions.Domain
                            },
                            SecretName = "tls-secret"
                        }
                    },
                    Rules = new InputList<IngressRuleArgs>
                    {
                        new IngressRuleArgs
                        {
                            Host = clusterOptions.Domain,
                            Http = new HTTPIngressRuleValueArgs
                            {
                                Paths = new InputList<HTTPIngressPathArgs>
                                {
                                    new HTTPIngressPathArgs
                                    {
                                        Path = "/api",
                                        Backend = new IngressBackendArgs
                                        {
                                            ServiceName = clusterOptions.AppointmentApi.ServiceName,
                                            ServicePort = clusterOptions.AppointmentApi.Port
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }, customOpts);
        }

        public static string GetMyPublicIpAddress()
        {
            using var wc = new WebClient();
            return wc.DownloadString("http://icanhazip.com");
        }

        private class AzureResourceBag
        {
            public ResourceGroup ResourceGroup { get; set; }
            public SqlServer SqlServer { get; set; }
            public KubernetesCluster Cluster { get; set; }
            public Insights AppInsights { get; set; }
            public InputMap<string> Tags { get; set; }
            public ServicePrincipal AksServicePrincipal { get; set; }
            public Subnet Subnet { get; set; }
            public Provider ClusterProvider { get; set; }
            public Registry Registry { get; set; }
        }

        private class AppointmentApiAzureResourceBag
        {
            public UserAssignedIdentity Identity { get; set; }
            public KeyVault KeyVault { get; set; }
        }
    }
}
