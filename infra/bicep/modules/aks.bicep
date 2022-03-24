@description('Specifies the name of the AKS cluster.')
@minLength(1)
@maxLength(63)
param aksClusterName string

@description('Specifies the tier of the AKS cluster SKU.')
@allowed([
  'Paid'
  'Free'
])
param aksClusterSku string

@description('Specifies the version of Kubernetes specified when creating the managed cluster.')
param aksClusterKubernetesVersion string

@description('Specifies the DNS prefix specified when creating the managed cluster.')
param aksClusterDnsPrefix string = aksClusterName

@description('Specifies the location of the AKS cluster.')
param location string = resourceGroup().location

@description('Specifies the network policy used for building Kubernetes network.')
@allowed([
  'azure'
  'calico'
])
param aksClusterNetworkPolicy string = 'azure'

@description('Specifies the network plugin used for building Kubernetes network.')
@allowed([
  'azure'
  'kubenet'
  'none'
])
param aksClusterNetworkPlugin string = 'azure'

@description('Specifies the CIDR notation IP range from which to assign pod IPs when kubenet is used.')
param aksClusterPodCidr string

@description('A CIDR notation IP range from which to assign service cluster IPs. It must not overlap with any Subnet IP ranges.')
param aksClusterServiceCidr string

@description('Specifies the IP address assigned to the Kubernetes DNS service. It must be within the Kubernetes service address range specified in serviceCidr.')
param aksClusterDnsServiceIP string

@description('Specifies the CIDR notation IP range assigned to the Docker bridge network. It must not overlap with any Subnet IP ranges or the Kubernetes service address range.')
param aksClusterDockerBridgeCidr string

@description('Specifies the sku of the load balancer used by the virtual machine scale sets used by nodepools.')
@allowed([
  'Basic'
  'Standard'
])
param aksClusterLoadBalancerSku string = 'Standard'

@description('Specifies outbound (egress) routing method. - loadBalancer or userDefinedRouting.')
@allowed([
  'loadBalancer'
  'userDefinedRouting'
])
param aksClusterOutboundType string = 'loadBalancer'

@description('Specifies the administrator username of Linux virtual machines.')
param aksClusterAdminUsername string

@description('Specifies the SSH RSA public key string for the Linux nodes.')
param aksClusterSshPublicKey string

@description('Specifies the id of the subnet hosting the worker nodes of the AKS cluster.')
param aksSubnetId string

@description('Specifies the id of the subnet hosting the pods of the AKS cluster.')
param podSubnetId string

@description('Specifies the unique name of of the system node pool profile. Must be unique in the context of the subscription and resource group.')
param systemNodePoolName string = 'system'

@description('Specifies the number of agents (VMs) to host docker containers in the system node pool.')
@minValue(1)
@maxValue(100)
param systemNodePoolAgentCount int = 3

@description('Specifies the VM size of nodes in the system node pool.')
param systemNodePoolVmSize string = 'Standard_D2_v3'

@description('Specifies the OS type for the vms in the system node pool. Choose from Linux and Windows.')
@allowed([
  'Windows'
  'Linux'
])
param systemNodePoolOsType string = 'Linux'

@description('Specifies the OS Disk Size in GB to be used to specify the disk size for every machine in the system agent pool. Passing 0 will cause the default size to be applied.')
@minValue(0)
@maxValue(1023)
param systemNodePoolOsDiskSizeGB int = 100

@description('Specifies the OS disk type to be used for machines in a given agent pool.')
@allowed([
  'Ephemeral'
  'Managed'
])
param systemNodePoolOsDiskType string = 'Ephemeral'

@description('Specifies the maximum number of pods that can run on a node in the system node pool.')
@maxValue(250)
param systemNodePoolMaxPods int = 30

@description('Specifies whether to enable auto-scaling for the system node pool.')
param systemNodePoolEnableAutoScaling bool = true

@description('Specifies the minimum number of nodes for auto-scaling for the system node pool.')
param systemNodePoolMinCount int = 3

@description('Specifies the maximum number of nodes for auto-scaling for the system node pool.')
param systemNodePoolMaxCount int = 5

@description('Specifies the virtual machine scale set priority in the system node pool.')
@allowed([
  'Spot'
  'Regular'
])
param systemNodePoolScaleSetPriority string = 'Regular'

@description('Specifies the ScaleSetEvictionPolicy to be used to specify eviction policy for spot virtual machine scale set.')
@allowed([
  'Delete'
  'Deallocate'
])
param systemNodePoolScaleSetEvictionPolicy string = 'Delete'

@description('Specifies the type for the system node pool.')
@allowed([
  'VirtualMachineScaleSets'
  'AvailabilitySet'
])
param systemNodePoolType string = 'VirtualMachineScaleSets'

@description('Specifies the unique name of of the user node pool profile. Must be unique in the context of the subscription and resource group.')
param userNodePoolName string = 'user'

@description('Specifies the number of agents (VMs) to host docker containers in the user node pool.')
@minValue(1)
@maxValue(100)
param userNodePoolAgentCount int = 3

@description('Specifies the VM size of nodes in the user node pool.')
param userNodePoolVmSize string = 'Standard_D2_v3'

@description('Specifies the OS type for the vms in the user node pool. Choose from Linux and Windows.')
@allowed([
  'Windows'
  'Linux'
])
param userNodePoolOsType string = 'Linux'

@description('Specifies the OS Disk Size in GB to be used to specify the disk size for every machine in the system agent pool. Passing 0 will cause the default size to be applied.')
@minValue(0)
@maxValue(1023)
param userNodePoolOsDiskSizeGB int = 100

@description('Specifies the OS disk type to be used for machines in a given agent pool.')
@allowed([
  'Ephemeral'
  'Managed'
])
param userNodePoolOsDiskType string = 'Ephemeral'

@description('Specifies the maximum number of pods that can run on a node in the user node pool.')
@maxValue(250)
param userNodePoolMaxPods int = 30

@description('Specifies whether to enable auto-scaling for the user node pool.')
param userNodePoolEnableAutoScaling bool = true

@description('Specifies the minimum number of nodes for auto-scaling for the user node pool.')
param userNodePoolMinCount int = 3

@description('Specifies the maximum number of nodes for auto-scaling for the user node pool.')
param userNodePoolMaxCount int = 5

@description('Specifies the virtual machine scale set priority in the user node pool.')
@allowed([
  'Spot'
  'Regular'
])
param userNodePoolScaleSetPriority string = 'Regular'

@description('Specifies the ScaleSetEvictionPolicy to be used to specify eviction policy for spot virtual machine scale set.')
@allowed([
  'Delete'
  'Deallocate'
])
param userNodePoolScaleSetEvictionPolicy string = 'Delete'

@description('Specifies the type for the user node pool.')
@allowed([
  'VirtualMachineScaleSets'
  'AvailabilitySet'
])
param userNodePoolType string = 'VirtualMachineScaleSets'

@description('Specifies the ID of the Log Analytics Workspace.')
param logAnalyticsWorkspaceId string

@description('Specifies the ID of the Contributor role.')
param contributorRoleId string = 'b24988ac-6180-42a0-ab88-20f7382dd24c'


// resources
var aksClusterUserDefinedManagedIdentityName = '${aksClusterName}-identity'
resource managedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' = {
  name: aksClusterUserDefinedManagedIdentityName
  location: location
}

resource contributorRole 'Microsoft.Authorization/roleDefinitions@2018-01-01-preview' existing = {
  name: contributorRoleId
  scope: subscription()
}

resource contributorRoleAssignment 'Microsoft.Authorization/roleAssignments@2020-10-01-preview' = {
  name: guid(resourceGroup().id, aksClusterUserDefinedManagedIdentityName, aksClusterName)
  properties: {
    roleDefinitionId: contributorRole.id
    principalId: managedIdentity.id
    principalType: 'ServicePrincipal'
  }
}

resource aks 'Microsoft.ContainerService/managedClusters@2022-01-02-preview' = {
  name: aksClusterName
  location: location
  sku: {
    name: 'Basic'
    tier: aksClusterSku
  }
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      aksClusterUserDefinedManagedIdentityId: {}
    }
  }
  properties: {
    kubernetesVersion: aksClusterKubernetesVersion
    dnsPrefix: aksClusterDnsPrefix
    agentPoolProfiles: [
      {
        name: systemNodePoolName
        count: systemNodePoolAgentCount
        vmSize: systemNodePoolVmSize
        osDiskType: systemNodePoolOsDiskType
        osDiskSizeGB: systemNodePoolOsDiskSizeGB
        osType: systemNodePoolOsType
        vnetSubnetID: aksSubnetId
        podSubnetID: podSubnetId
        maxPods: systemNodePoolMaxPods
        minCount: systemNodePoolMinCount
        maxCount: systemNodePoolMaxCount
        scaleSetPriority: systemNodePoolScaleSetPriority
        scaleSetEvictionPolicy: systemNodePoolScaleSetEvictionPolicy
        enableAutoScaling: systemNodePoolEnableAutoScaling
        type: systemNodePoolType
        mode: 'System'
      }
      {
        name: userNodePoolName
        count: userNodePoolAgentCount
        vmSize: userNodePoolVmSize
        osDiskType: userNodePoolOsDiskType
        osDiskSizeGB: userNodePoolOsDiskSizeGB
        osType: userNodePoolOsType
        vnetSubnetID: aksSubnetId
        podSubnetID: podSubnetId
        maxPods: userNodePoolMaxPods
        minCount: userNodePoolMinCount
        maxCount: userNodePoolMaxCount
        scaleSetPriority: userNodePoolScaleSetPriority
        scaleSetEvictionPolicy: userNodePoolScaleSetEvictionPolicy
        enableAutoScaling: userNodePoolEnableAutoScaling
        type: userNodePoolType
        mode: 'User'
      }
    ]
    linuxProfile: {
      adminUsername: aksClusterAdminUsername
      ssh: {
        publicKeys: [
          {
            keyData: aksClusterSshPublicKey
          }
        ]
      }
    }
    enableRBAC: true
    networkProfile: {
      networkPlugin: aksClusterNetworkPlugin
      networkPolicy: aksClusterNetworkPolicy
      podCidr: aksClusterPodCidr
      serviceCidr: aksClusterServiceCidr
      dnsServiceIP: aksClusterDnsServiceIP
      dockerBridgeCidr: aksClusterDockerBridgeCidr
      outboundType: aksClusterOutboundType
      loadBalancerSku: aksClusterLoadBalancerSku
    }
    addonProfiles: {
      omsagent: {
        config: {
          logAnalyticsWorkspaceResourceID: logAnalyticsWorkspaceId
        }
        enabled: true
      }
    }
  }
}
