@description('The name of the managed identity resource.')
param managedIdentityName string

@description('Roles to assign.')
param assignedRoleIds array

@description('The location of the managed identity resource.')
param location string

// resources
resource managedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' = {
  name: managedIdentityName
  location: location
}

resource roleAssignment 'Microsoft.Authorization/roleAssignments@2020-10-01-preview' = [for id in assignedRoleIds: {
  name: guid(subscription().id, managedIdentityName, id)
  properties: {
    principalId: managedIdentity.properties.principalId
    roleDefinitionId: id
    principalType: 'ServicePrincipal'
  }
}]

output id string = managedIdentity.id
