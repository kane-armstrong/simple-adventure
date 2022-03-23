# Bicep Notes

## Creating application registrations

Why this matters:

I don't know if this is strictly required for creating RBAC-enabled Kuberenetes clusters, but the way I know
how to (right now) is by creating an app registration, creating a service principal for that registration,
giving that service principal role assignments over certain resources (e.g. Network Contributor over a subnet
so that the SP can provision LBs, or AcrPull over a container registry so that images can be pulled from it
without needing to maintain credential configuration), then configuring the AKS cluster to use this SP.

If you're automating the deployment of your Azure resources, then the identity used to deploy these resources
needs `Application administrator` role membership (perhaps there is a better option from a least-privilege
perspective; still need to check) in order to create the application registration and apply the necessary
role assignments.

### Problem 1

Bicep doesn't support assigning AAD built-in roles to managed identities (e.g. roles with microsoft.directory
actions, like Application Administrator). Attempting to create a role assignment for an existing role requires
something like this:

```bicep
resource networkContributorRoleDefinition 'Microsoft.Authorization/roleDefinitions@2018-01-01-preview' existing = {
  name: '4d97b98b-1d4f-4787-a291-c67834d212e7'
  scope: subscription()
}
```

Note the use of `existing` (denoting that the role definition already exists).

The problem is that `az deploy` complains that the role definition was not found when using built-in AAD roles. The
following example will reproduce the error.

```bicep
resource applicationAdministratorRoleDefinition 'Microsoft.Authorization/roleDefinitions@2018-01-01-preview' existing = {
  name: '9b895d92-2cd3-44c7-9d02-a6ac2d5ea5c3'
  scope: subscription()
}
```

Note that these roles, their identifiers, and their action lists can be found [here](https://docs.microsoft.com/en-us/azure/active-directory/roles/permissions-reference).

### Problem 2

I tried to get around the previous problem by provisioning a custom role, giving said role the same actions as the built-in
AAD roles, and assigning that custom role to my managed identity.

Example module:

```bicep
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
```

Example usage:

```bicep
module customRoleModule './modules/customRole.bicep' = {
  name: 'customRoleDeploy'
  scope: subscription()
  params: {
    roleName: 'Application developer'
    roleDescription: 'Create application registrations in AAD'
    actions: [
      'microsoft.directory/applications/createAsOwner'
      'microsoft.directory/oAuth2PermissionGrants/createAsOwner'
      'microsoft.directory/servicePrincipals/createAsOwner'
    ]
    notActions: [
      
    ]
  }
}
```

This failed as the actions used by the AAD built-in roles are not known to bicep.

```json
{
    "code": "InvalidActionOrNotAction",
    "message": "The resource provider referenced in the action 'microsoft.directory/applications/createAsOwner' is not returned in the list of providers from Azure Resource Manager."
}
```

There might be a way around this, but I stopped here and went with `az` instead.

### Granting role assignments to built-in AAD roles via az cli

I found the documentation for this [here](https://github.com/microsoftgraph/microsoft-graph-docs/blob/main/api-reference/beta/api/rbacapplication-post-roleassignments.md#http)

```powershell
$payload = "{\`"principalId\`": \`"${principalId}\`", \`"roleDefinitionId\`": \`"${roleId}\`", \`"directoryScopeId\`": \`"/\`" }"
az rest --method POST --uri 'https://graph.microsoft.com/beta/roleManagement/directory/roleAssignments' --body $payload --headers "content-type=application/json"
```
