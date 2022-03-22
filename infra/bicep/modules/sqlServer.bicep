@description('The name of the SQL server.')
@minLength(1)
@maxLength(63)
param sqlServerName string

@description('The administrator username of the SQL server.')
param sqlAdministratorLogin string

@description('The administrator password of the SQL server.')
@secure()
param sqlAdministratorLoginPassword string

@description('The version of the SQL server.')
param sqlServerVersion string

@description('The name of the VNet rule.')
@minLength(1)
@maxLength(128)
param virtualNetworkRuleName string

@description('The id of the assigned VNet subnet.')
param virtualNetworkRuleSubnetId string

@description('The location of the SQL Server resource.')
param location string


// resources
resource sqlServer 'Microsoft.Sql/servers@2021-08-01-preview' = {
  name: sqlServerName
  location: location
  properties: {
    administratorLogin: sqlAdministratorLogin
    administratorLoginPassword: sqlAdministratorLoginPassword
    version: sqlServerVersion
  }
}

resource sqlServerNetworkRule 'Microsoft.Sql/servers/virtualNetworkRules@2021-08-01-preview' = {
  name: virtualNetworkRuleName
  parent: sqlServer
  properties: {
    virtualNetworkSubnetId: virtualNetworkRuleSubnetId
  }
}
