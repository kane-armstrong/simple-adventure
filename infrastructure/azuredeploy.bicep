@description('Location of all resources')
param location string

@description('The type of environment resources are deployed to.')
@allowed([
  'Production'
  'Development'
])
param environmentType string = 'Development'

@description('The admin user name for the AKS cluster')
param aksClusterAdminUsername string

@description('The public SSH key for the AKS cluster')
param aksClusterSshPublicKey string

@description('The username of the admin account of the SQL Server resource')
param sqlServerAdminUsername string

@description('The password of the admin account of the SQL Server resource')
@secure()
param sqlServerAdminPassword string

@description('The version of the SQL Server resource.')
@secure()
param sqlServerVersion string

var environmentConfigurationMap = {
  Development: {
    environmentCode: 'dev'
    networks: {
      vnet: {
        ddosProtectionEnabled: false
      }
    }
    containerRegistry: {
      adminUserEnabled: true
      sku: 'Standard'
    }
    operationalInsights: {
      sku: 'PerGB2018'
      retention: 30
    }
    aks: {
      nodePools: {
        system: {
          agentCount: 3
          diskSize: 30
          vmSize: 'Standard_D2_v3'
        }
        user: {
          agentCount: 3
          diskSize: 30
          vmSize: 'Standard_D2_v3'          
        }
      }
    }
  }
}

var prefixes = json(loadTextContent('./shared-prefixes.json'))
var env = environmentConfigurationMap[environmentType].environmentCode

var rg = resourceGroup()

var acrName = '${prefixes.project}${env}${prefixes.azureContainerRegistry}${uniqueString(rg.id)}'
var vnetName = '${prefixes.project}-${env}-${prefixes.virtualNetwork}-${uniqueString(rg.id)}'
var workspaceName = '${prefixes.project}-${env}-${prefixes.operationalInsightsWorkspace}-${uniqueString(rg.id)}'
var sqlServerName = '${prefixes.project}-${env}-${prefixes.sqlServer}-${uniqueString(rg.id)}'
var sqlServerVirtualNetworkRuleName = guid(subscription().id, sqlServerName, vnetName)
var appInsightsName = '${prefixes.project}-${env}-${prefixes.appInsights}-${uniqueString(rg.id)}'
var aksClusterName = '${prefixes.project}-${env}-${prefixes.azureKubernetesService}-${uniqueString(rg.id)}'


var aksSubnetName = 'AksSubnet'
var podSubnetName = 'PodSubnet'

module networkModule './modules/network.bicep' = {
  name: 'networkDeploy'
  scope: rg
  params: {
    vnetName: vnetName
    vnetAddressPrefix: '10.0.0.0/8'
    vnetDdosProtectionEnabled: environmentConfigurationMap[environmentType].networks.vnet.ddosProtectionEnabled
    vnetSubnets: [
      {
        name: aksSubnetName
        addressPrefix: '10.0.0.0/16'
        serviceEndpoints: [
          {
            service: 'Microsoft.KeyVault'
          }
          {
            service: 'Microsoft.Sql'
          }
        ]
      }
    ]
    location: location
  }
}

module acrModule './modules/acr.bicep' = {
  name:'registryDeploy'
  scope: rg
  params: {
    acrName: acrName
    acrSku: environmentConfigurationMap[environmentType].containerRegistry.sku
    acrAdminUserEnabled: environmentConfigurationMap[environmentType].containerRegistry.adminUserEnabled    
    location: location
  }
}

module operationsInsightsModule './modules/operationalInsights.bicep' = {
  name: 'operationalInsightsDeploy'
  scope: rg
  params: {
    workspaceName: workspaceName
    workspaceSku: environmentConfigurationMap[environmentType].operationalInsights.sku
    workspaceRetentionDays: environmentConfigurationMap[environmentType].operationalInsights.retention
    appInsightsName: appInsightsName
    location: location
  }
}

module sqlServerModule './modules/sqlServer.bicep' = {
  name: 'sqlServerDeploy'
  scope: rg
  params: {
    sqlServerName: sqlServerName
    sqlAdministratorLogin: sqlServerAdminUsername
    sqlAdministratorLoginPassword: sqlServerAdminPassword
    sqlServerVersion: sqlServerVersion
    virtualNetworkName: vnetName
    virtualNetworkRuleName: sqlServerVirtualNetworkRuleName
    virtualNetworkRuleSubnetName: aksSubnetName
    location: location
  }
}

module aksModule './modules/aks.bicep' = {
  name: 'aksDeploy'
  scope: rg
  params: {
    aksClusterName: aksClusterName
    aksClusterSku: 'Paid'
    aksClusterKubernetesVersion: '1.22.6'
    aksClusterDnsPrefix: aksClusterName
    aksClusterPodCidr: '10.244.0.0/16'
    aksClusterServiceCidr: '10.3.0.0/16'
    aksClusterDnsServiceIP: '10.3.0.10'
    aksClusterDockerBridgeCidr: '172.17.0.1/16'
    aksClusterAdminUsername: aksClusterAdminUsername
    aksClusterSshPublicKey: aksClusterSshPublicKey
    systemNodePoolVmSize: environmentConfigurationMap[environmentType].aks.nodePools.system.vmSize
    systemNodePoolOsDiskSizeGB: environmentConfigurationMap[environmentType].aks.nodePools.system.diskSize
    systemNodePoolOsDiskType: 'Managed'
    systemNodePoolAgentCount: environmentConfigurationMap[environmentType].aks.nodePools.system.agentCount
    userNodePoolVmSize: environmentConfigurationMap[environmentType].aks.nodePools.user.vmSize
    userNodePoolOsDiskSizeGB: environmentConfigurationMap[environmentType].aks.nodePools.user.diskSize
    userNodePoolOsDiskType: 'Managed'
    userNodePoolAgentCount: environmentConfigurationMap[environmentType].aks.nodePools.user.agentCount
    virtualNetworkName: vnetName
    aksSubnetName: aksSubnetName
    logAnalyticsWorkspaceId: operationsInsightsModule.outputs.workspaceId
    location: location
  }
}
