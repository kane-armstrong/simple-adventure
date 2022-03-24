$location = "australiasoutheast"
$environmentCode = "dev"
$resourceGroupName = "pd-$environmentCode-rg"

az group create --name $resourceGroupName --location $location
az deployment group create `
    --resource-group $resourceGroupName `
    --template-file .\azuredeploy.bicep `
    --parameters .\azuredeploy.parameters.json