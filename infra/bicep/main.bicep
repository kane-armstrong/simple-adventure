targetScope = 'subscription'

@description('Location of all resources')
param location string

@description('The type of environment resources are deployed to.')
@allowed([
  'Production'
  'Development'
])
param environmentType string = 'Development'

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
      nodes: {
        count: 3
        vmSize: 'Standard_D2_v3'
        diskSizeGb: 30
      }
    }
  }
}

var prefixes = json(loadTextContent('./shared-prefixes.json'))
var env = environmentConfigurationMap[environmentType].environmentCode

var resourceGroupName = '${prefixes.project}-${env}-${prefixes.resourceGroup}'
resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  location: location
  name: resourceGroupName
}

var acrName = '${prefixes.project}${env}${prefixes.azureContainerRegistry}${uniqueString(rg.id)}'
var vnetName = '${prefixes.project}-${env}-${prefixes.virtualNetwork}-${uniqueString(rg.id)}'
var subnetName = '${prefixes.project}-${env}-${prefixes.subnet}-${uniqueString(rg.id)}'
var workspaceName = '${prefixes.project}-${env}-${prefixes.operationalInsightsWorkspace}-${uniqueString(rg.id)}'
var sqlServerName = '${prefixes.project}-${env}-${prefixes.sqlServer}-${uniqueString(rg.id)}'
var sqlServerVirtualNetworkRuleName = guid(subscription().id, sqlServerName, vnetName)
var appInsightsName = '${prefixes.project}-${env}-${prefixes.appInsights}-${uniqueString(rg.id)}'
var aksClusterName = '${prefixes.project}-${env}-${prefixes.azureKubernetesService}-${uniqueString(rg.id)}'
var aksManagedIdentityName = '${prefixes.project}-${prefixes.azureKubernetesService}-${prefixes.managedIdentity}-${uniqueString(rg.id)}'
var aksAppRegistrationName = '${prefixes.project}-${prefixes.azureKubernetesService}-${uniqueString(rg.id)}'

module networkModule './modules/network.bicep' = {
  name: 'networkDeploy'
  scope: rg
  params: {
    vnetName: vnetName
    vnetAddressPrefix: '10.0.0.0/8'
    vnetDdosProtectionEnabled: environmentConfigurationMap[environmentType].networks.vnet.ddosProtectionEnabled
    subnetName: subnetName
    subnetPrefix: '10.240.0.0/16'
    subnetServiceEndpoints: [
      {
        service: 'Microsoft.KeyVault'
      }
      {
        service: 'Microsoft.Sql'
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
    sqlAdministratorLogin: ''
    sqlAdministratorLoginPassword: ''
    sqlServerVersion: '12.0'
    virtualNetworkRuleName: sqlServerVirtualNetworkRuleName
    virtualNetworkRuleSubnetId: networkModule.outputs['subnetId']
    location: location
  }
}

module aksAppRegistration './modules/appRegistration.bicep' = {
  name: 'aksAppRegistrationDeploy'
  scope: rg
  params: {
    appRegistrationName: aksAppRegistrationName
    managedIdentityName: aksManagedIdentityName
    location: location
  }
}

resource networkContributorRoleDefinition 'Microsoft.Authorization/roleDefinitions@2018-01-01-preview' existing = {
  name: '4d97b98b-1d4f-4787-a291-c67834d212e7'
  scope: subscription()
}

resource acrPullRoleDefinition 'Microsoft.Authorization/roleDefinitions@2018-01-01-preview' existing = {
  name: '7f951dda-4ed3-4680-a7ca-43fe172d538d'
  scope: subscription()
}

resource subnetAssignment 'Microsoft.Authorization/roleAssignments@2020-10-01-preview' = {
  name: guid(subscription().id, aksAppRegistrationName, networkContributorRoleDefinition.id)
  properties: {
    principalId: aksAppRegistration.outputs['principalId']
    roleDefinitionId: networkContributorRoleDefinition.id
    principalType: 'ServicePrincipal'
  }
}

resource acrAssignment 'Microsoft.Authorization/roleAssignments@2020-10-01-preview' = {
  name: guid(subscription().id, aksAppRegistrationName, acrPullRoleDefinition.id)
  properties: {
    principalId: aksAppRegistration.outputs['principalId']
    roleDefinitionId: acrPullRoleDefinition.id
    principalType: 'ServicePrincipal'
  }
}

module aksModule './modules/aks.bicep' = {
  name: 'aksDeploy'
  scope: rg
  params: {
    clusterName: aksClusterName    
    dnsPrefix: 'dns'
    kubernetesVersion: '1.22.6'
    linuxAdminUsername: 'aksuser'
    sshRSAPublicKey: ''
    nodeCount: environmentConfigurationMap[environmentType].aks.nodes.count    
    nodeVMSize: environmentConfigurationMap[environmentType].aks.nodes.vmSize
    osDiskSizeGB: environmentConfigurationMap[environmentType].aks.nodes.diskSizeGb
    virtualNetworkSubnetId: networkModule.outputs['subnetId']
    networkServiceCidr: '10.2.0.0/24'
    networkDnsServiceIP: '10.2.0.10'
    networkDockerBridgeCidr: '172.17.0.1/16'
    rbacEnabled: true
    clusterServicePrincipalClientId: aksAppRegistration.outputs['clientId']
    clusterServicePrincipalClientSecret: aksAppRegistration.outputs['clientSecret']
    operationalInsightsWorkspaceId: operationsInsightsModule.outputs['workspaceId']
    location: location
  }
}
