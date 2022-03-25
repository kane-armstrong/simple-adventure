@description('Specifies the name of the application the deployed resources are associated with.')
param applicationName string

@description('Specifies the location of all resources.')
param location string

@description('Specifies the type of environment resources are deployed to.')
@allowed([
  'Production'
  'Development'
])
param environmentType string = 'Development'

@description('Specifies the admin user name for the AKS cluster.')
param aksClusterAdminUsername string

@description('Specifies the public SSH key for the AKS cluster.')
param aksClusterSshPublicKey string

@description('Specifies the username of the admin account of the SQL server.')
param sqlServerAdminUsername string

@description('Specifies the password of the admin account of the SQL server.')
@secure()
param sqlServerAdminPassword string

@description('Specifies the version of the SQL server.')
@secure()
param sqlServerVersion string


// variables
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

var locationMap = {
  'australiasoutheast': 'ause'
  'australiaeast': 'aue'
  'westus': 'usw'
  'eastus': 'use'
  'centralus': 'usc'
}

var prefixes = json(loadTextContent('./shared-prefixes.json'))
var env = environmentConfigurationMap[environmentType].environmentCode

var nameSuffix = '${applicationName}-${env}-${locationMap[location]}'

var acrName = replace('${prefixes.azureContainerRegistry}-${nameSuffix}', '-', '')
var vnetName = '${prefixes.virtualNetwork}-${nameSuffix}'
var workspaceName = '${prefixes.operationalInsightsWorkspace}-${nameSuffix}'
var sqlServerName = '${prefixes.sqlServer}-${nameSuffix}'
var appInsightsName = '${prefixes.appInsights}-${nameSuffix}'
var aksClusterName = '${prefixes.azureKubernetesService}-${nameSuffix}'

var aksSubnetName = 'AksSubnet'


// resources
module networkModule './modules/network.bicep' = {
  name: 'networkDeploy'
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
  params: {
    acrName: acrName
    acrSku: environmentConfigurationMap[environmentType].containerRegistry.sku
    acrAdminUserEnabled: environmentConfigurationMap[environmentType].containerRegistry.adminUserEnabled    
    location: location
  }
}

module operationsInsightsModule './modules/operationalInsights.bicep' = {
  name: 'operationalInsightsDeploy'
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
  params: {
    sqlServerName: sqlServerName
    sqlAdministratorLogin: sqlServerAdminUsername
    sqlAdministratorLoginPassword: sqlServerAdminPassword
    sqlServerVersion: sqlServerVersion
    virtualNetworkName: vnetName
    subnetName: aksSubnetName
    location: location
  }
}

module aksModule './modules/aks.bicep' = {
  name: 'aksDeploy'
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
