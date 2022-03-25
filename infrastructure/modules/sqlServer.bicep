@description('Specifies the name of the SQL server.')
@minLength(1)
@maxLength(63)
param sqlServerName string

@description('Specifies the administrator username of the SQL server.')
param sqlAdministratorLogin string

@description('Specifies the administrator password of the SQL server.')
@secure()
param sqlAdministratorLoginPassword string

@description('Specifies the version of the SQL server.')
param sqlServerVersion string

@description('Specifies the name of the virtual network hosting the SQL server.')
param virtualNetworkName string

@description('Specifies the name of the subnet hosting the SQL server.')
param subnetName string

@description('Specifies the location of the SQL server.')
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
  name: guid(subscription().id, sqlServerName, virtualNetworkName)
  parent: sqlServer
  properties: {
    virtualNetworkSubnetId: resourceId('Microsoft.Network/virtualNetworks/subnets', virtualNetworkName, subnetName)
  }
}
