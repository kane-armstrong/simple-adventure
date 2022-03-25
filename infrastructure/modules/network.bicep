@description('Specifies the name of the virtual network.')
@minLength(2)
@maxLength(64)
param vnetName string

@description('Specifies the address prefix of the virtual network.')
param vnetAddressPrefix string

@description('Specifies whether to enable DDoS protection for the virtual network.')
param vnetDdosProtectionEnabled bool = true

param vnetSubnets array

@description('Specifies the location of the virtual network.')
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
}
