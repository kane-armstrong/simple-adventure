@description('The VNet name')
@minLength(2)
@maxLength(64)
param vnetName string

@description('The VNet address prefix')
param vnetAddressPrefix string

@description('Enable VNet DDoS protection.')
param vnetDdosProtectionEnabled bool = true

@description('The subnet name')
@minLength(1)
@maxLength(80)
param subnetName string

@description('The subnet prefix')
param subnetPrefix string

@description('Service Endpoints for the subnet.')
param subnetServiceEndpoints array

@description('The location of the network resources.')
param location string


// resources

resource subnet 'Microsoft.Network/virtualNetworks/subnets@2021-05-01' = {
  name: subnetName
  properties: {
    addressPrefixes: [ 
      subnetPrefix
    ]
    serviceEndpoints: subnetServiceEndpoints
  }
}

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
    subnets: [
      subnet
    ]
  }
}

output subnetId string = subnet.id
