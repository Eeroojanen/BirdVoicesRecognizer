@description('Provide a name for the Key Vault. The name must be globally unique.')
param keyVaultName string = 'keyVault${uniqueString(resourceGroup().id)}'

@description('Location of the Key Vault resource.')
param keyVaultLocation string = resourceGroup().location

@description('Provide a name for the storage account. Use only lowercase letters and numbers. The name must be unique across Azure.')
param birdVoiceStorage string = 'store${uniqueString(resourceGroup().id)}'

@description('Location of the resources.')
param location string = resourceGroup().location  

@description('Provide a name for the Cosmos DB account. This must be globally unique.')
param cosmosDbDatabase string = 'cosmosdb${uniqueString(resourceGroup().id)}'

@description('Container name for birdvoice metadata.')
param audioFiles string = 'audioFiles'

@description('Container for audio files analysis.')
param audioFileAnalysis string = 'audioFileAnalysis'

@description('Name for function app')
param functionAppName string = 'mp3-analysis-func-${uniqueString(resourceGroup().id)}'

module functionAppModule 'functionapp.bicep' = {
  name: 'functionAppDeployment'
  params: {
    functionAppName: functionAppName
    location: location
    appInsightsLocation: location
    functionWorkerRuntime: 'dotnet'
    keyVaultName: keyVaultName
  }
}

module storageModule 'storage.bicep' = {
  name: 'storageDeployment'
  params: {
    birdVoiceStorage: birdVoiceStorage
    location: location
  }
}

module cosmosDbModule 'cosmosdb.bicep' = {
  name: 'cosmosDbDeployment'
  params: {
    cosmosDbDatabase: cosmosDbDatabase
    location: resourceGroup().location
    audioFiles: audioFiles
    audioFileAnalysis: audioFileAnalysis
  }
}

module keyVaultModule 'keyvault.bicep' = {
  name: 'keyVaultDeployment'
  params: {
    keyVaultName: keyVaultName
    keyVaultLocation: keyVaultLocation
    storageAccountConnectionString: 'DefaultEndpointsProtocol=https;AccountName=${storageModule.outputs.storageName};AccountKey=${storageModule.outputs.storageKey}'
    cosmosDbConnectionString: 'AccountEndpoint=https://${cosmosDbModule.outputs.accountName}.documents.azure.com:443/;AccountKey=${cosmosDbModule.outputs.accountKey};'
    functionAppPrincipalId: functionAppModule.outputs.principalId
  }
}

output keyVaultUri string = keyVaultModule.outputs.keyVaultUri
output containerName string = storageModule.outputs.containerName
output functionAppNameOutput string = functionAppModule.outputs.functionAppNameOutput
output storageAccountNameOutput string = functionAppModule.outputs.storageAccountNameOutput
output applicationInsightsNameOutput string = functionAppModule.outputs.applicationInsightsNameOutput
