@description('The name of the managed cluster.')
@minLength(1)
@maxLength(63)
param clusterName string

@description('Optional DNS prefix to use with hosted Kubernetes API server FQDN.')
param dnsPrefix string

@description('The Kubernetes version of the managed cluster.')
param kubernetesVersion string

@description('The Client ID of the managed cluster service principal.')
param clusterServicePrincipalClientId string

@description('The Client Secret of the managed cluster service principal.')
param clusterServicePrincipalClientSecret string

@description('Enable role-based access control.')
param rbacEnabled bool = true

@description('Disk size (in GB) to provision for each of the agent pool nodes. Specifying 0 will apply the default disk size for that agentVMSize.')
@minValue(0)
@maxValue(1023)
param osDiskSizeGB int = 0

@description('The initial number of nodes which should exist in the Node Pool.')
@minValue(1)
@maxValue(50)
param nodeCount int = 3

@description('The size of the each Virtual Machine node.')
param nodeVMSize string = 'Standard_Ds_v3'

@description('The Admin username for the cluster.')
param linuxAdminUsername string

@description('The Public SSH Key used to access the cluster.')
param sshRSAPublicKey string

@description('The Network Range used by the Kubernetes service.')
param networkServiceCidr string

@description('The IP address within the Kubernetes service address range that will be used by cluster service discovery (kube-dns).')
param networkDnsServiceIP string

@description('The IP address (in CIDR notation) used as the Docker bridge IP address on nodes.')
param networkDockerBridgeCidr string

@description('The id of the VNet subnet.')
param virtualNetworkSubnetId string

@description('The ID of the Log Analytics Workspace which the OMS Agent should send data to.')
param operationalInsightsWorkspaceId string

@description('The location of the managed cluster resource.')
param location string


// resources
resource aks 'Microsoft.ContainerService/managedClusters@2022-01-02-preview' = {
  name: clusterName
  location: location
  properties: {
    dnsPrefix: dnsPrefix
    kubernetesVersion: kubernetesVersion
    enableRBAC: rbacEnabled
    autoScalerProfile: {

    }
    agentPoolProfiles: [
      {
        name: 'agentpool'
        osDiskSizeGB: osDiskSizeGB
        count: nodeCount
        vmSize: nodeVMSize
        osType: 'Linux'
        vnetSubnetID: virtualNetworkSubnetId
      }
    ]
    linuxProfile: {
      adminUsername: linuxAdminUsername
      ssh: {
        publicKeys: [
          {
            keyData: sshRSAPublicKey
          }
        ]
      }
    }
    servicePrincipalProfile: {
      clientId: clusterServicePrincipalClientId
      secret: clusterServicePrincipalClientSecret
    }
    networkProfile: {
      networkPlugin: 'azure'
      serviceCidr: networkServiceCidr
      dnsServiceIP: networkDnsServiceIP
      dockerBridgeCidr: networkDockerBridgeCidr
    }
    addonProfiles: {
      omsagent: {
        config: {
          logAnalyticsWorkspaceResourceID: operationalInsightsWorkspaceId
        }
        enabled: true
      }
    }
  }
}
