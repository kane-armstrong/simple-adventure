@description('Specifies the name of the container registry (must be globally unique).')
@minLength(5)
@maxLength(50)
param acrName string

@description('Specifies whether to enable an admin user.')
param acrAdminUserEnabled bool = false

@description('Specifies the tier of the container registry SKU.')
@allowed([
  'Basic'
  'Classic'
  'Standard'
  'Premuim'
])
param acrSku string = 'Basic'

@description('Specifies the location of the container registry.')
param location string


// resources
resource acr 'Microsoft.ContainerRegistry/registries@2021-09-01' = {
  name: acrName
  location: location
  sku: {
    name: acrSku
  }
  properties: {
    adminUserEnabled: acrAdminUserEnabled
  }
}

output acrLoginServer string = acr.properties.loginServer
