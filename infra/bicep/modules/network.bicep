@description('The VNet name')
@minLength(2)
@maxLength(64)
param vnetName string

@description('The VNet address prefix')
param vnetAddressPrefix string

@description('Enable VNet DDoS protection.')
param vnetDdosProtectionEnabled bool = true

param vnetSubnets array

@description('The location of the network resources.')
param location string


// resources
resource virtualNetwork 'Microsoft.Network/virtualNetworks@2021-05-01' = {
  name: vnetName
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        vnetAddressPrefix
      ]
    }
    enableDdosProtection: vnetDdosProtectionEnabled
    subnets: [for subnet in vnetSubnets: {
      name: subnet.name
      properties: {
        addressPrefix: subnet.addressPrefix
        serviceEndpoints: subnet.serviceEndpoints
      }
    }]
  }

  resource subnet 'subnets' existing = [for subnet in vnetSubnets: {
    name: subnet.name
  }]
}
