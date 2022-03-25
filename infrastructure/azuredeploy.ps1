Param(
    $location = "australiasoutheast",
    $environmentCode = "dev",
    $resourceGroupName = "pd-$environmentCode-rg"
)

az group create --name $resourceGroupName --location $location
az deployment group create `
    --resource-group $resourceGroupName `
    --template-file .\shared\azuredeploy.bicep `
    --parameters .\shared\azuredeploy.parameters.json