@description('The name of the registry (must be globally unique).')
@minLength(5)
@maxLength(50)
param acrName string

@description('Enable an admin user with push/pull access to the registry.')
param acrAdminUserEnabled bool = true

@description('The tier of the Azure Container Registry resource.')
@allowed([
  'Basic'
  'Classic'
  'Standard'
  'Premuim'
])
param acrSku string = 'Basic'

@description('The location of the Azure Container Registry resource.')
param location string


// resources
resource acr 'Microsoft.ContainerRegistry/registries@2021-09-01' = {
  name: acrName
  location: location
  sku: {
#disable-next-line BCP036
    name: acrSku
  }
  properties: {
    adminUserEnabled: acrAdminUserEnabled
  }
}

output acrLoginServer string = acr.properties.loginServer
