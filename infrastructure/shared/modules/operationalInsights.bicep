@description('Specifies the name of the log analytics workspace.')
@minLength(4)
@maxLength(63)
param workspaceName string

@description('Specifies the tier of the log analytics workspace SKU.')
@allowed([
  'PerGB2018'
  'CapacityReservation'
  'Free'
  'LACluster'
  'PerNode'
  'Premuim'
  'Standalone'
  'Standard'
])
param workspaceSku string = 'PerGB2018'

@description('Specifies the data retention period (in days) of the log analytics workspace.')
@minValue(30)
@maxValue(730)
param workspaceRetentionDays int

@description('Specifies the name of the application insights resource.')
@minLength(1)
@maxLength(260)
param appInsightsName string

@description('Specifies the location of the log analytics workspace.')
param location string


//resources
resource workspace 'Microsoft.OperationalInsights/workspaces@2021-06-01' = {
  name: workspaceName
  location: location
  properties: {
    sku: {
      name: workspaceSku
    }
    retentionInDays: workspaceRetentionDays
  }
}

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: workspace.id
  }
}

var containerInsightsSolutionName = 'ContainerInsights(${workspaceName})'
resource containerInsightsSolution 'Microsoft.OperationsManagement/solutions@2015-11-01-preview' = {
  name: containerInsightsSolutionName
  location: location
  plan: {
    name: containerInsightsSolutionName
    product: 'OMSGallery/ContainerInsights'
    publisher: 'Microsoft'
    promotionCode: ''
  }
  properties: {
    workspaceResourceId: workspace.id
  }
}

output workspaceId string = workspace.id
