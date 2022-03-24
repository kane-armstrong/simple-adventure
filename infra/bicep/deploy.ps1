$location = "australiasoutheast"
$sqluser = ""
$sqlpassword = ""
$environmentCode = "dev"
$managedIdentityName = "pd-app-registration"
$resourceGroupName = "pd-$environmentCode-rg"
$applicationAdministratorRoleId = "9b895d92-2cd3-44c7-9d02-a6ac2d5ea5c3"
$aksUserName = ""
$sshKey = ""

function Grant-RoleAssignment($principalId, $roleId) {
    # https://github.com/microsoftgraph/microsoft-graph-docs/blob/main/api-reference/beta/api/rbacapplication-post-roleassignments.md#http
    $payload = "{\`"principalId\`": \`"${principalId}\`", \`"roleDefinitionId\`": \`"${roleId}\`", \`"directoryScopeId\`": \`"/\`" }"
    az rest --method POST --uri 'https://graph.microsoft.com/beta/roleManagement/directory/roleAssignments' --body $payload --headers "content-type=application/json"
}

function CreateResourceGroupIfNotExists($name) {
    if((az group exists -n $name) -eq "false") {
        Write-Host Resource group does not exist - creating
        az group create --name $name --location $location
    } else {
        Write-Host Resource group has already been created - skipping
    }
}

function CreateManagedIdentityIfNotExists($group, $name) {
    $identity = az identity show -g $group -n $name | ConvertFrom-Json
    if ($identity -eq $null) {
        Write-Host Managed identity does not exist - creating
        $identity = (az identity create --name $managedIdentityName) | ConvertFrom-Json
    } else {
        Write-Host Managed identity has already been created - skipping
    }
    $identity
}

CreateResourceGroupIfNotExists -name $resourceGroupName
$identity = CreateManagedIdentityIfNotExists -group $resourceGroupName -name $managedIdentityName

# probably need some kind of delay to give things time to replicate... running this immediately after principal creation leads to 404s
Write-Host Granting $applicationAdministratorRoleId role assigned to managed identity
$principalId = $identity.principalId
Grant-RoleAssignment -principalId $principalId -roleId $applicationAdministratorRoleId

$managedIdentityId = $identity.id
$managedIdentityPrincipalId = $identity.principalId
az deployment group create `
    --resource-group $resourceGroupName `
    --template-file .\main.bicep `
    --parameters `
        location=$location `
        sqlAdminUserName=$sqluser `
        sqlAdminPassword=$sqlpassword `
        appRegistrationManagedIdentityId=$managedIdentityId `
        appRegistrationManagedIdentityPrincipalId=$managedIdentityPrincipalId `
        aksUserName=$aksUserName `
        aksPubSshKey=$sshKey