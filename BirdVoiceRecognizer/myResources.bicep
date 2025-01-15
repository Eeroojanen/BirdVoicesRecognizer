@minLength(3)
@maxLength(24)
@description('Provide a name for the storage account. Use only lowercase letters and numbers. The name must be unique across Azure.')
param birdVoiceStorage string = 'store${uniqueString(resourceGroup().id)}'
param cosmosDbDatabase string = 'cosmosdb${uniqueString(resourceGroup().id)}'
param location string = resourceGroup().location

resource birdVoices 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: birdVoiceStorage
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'BlobStorage'
}

resource cosmosDb 'Microsoft.DocumentDb/databaseAccounts@2023-05-01' = {
  name: cosmosDbDatabase
  location: location
  kind: 'GlobalDocumentDB'
  properties: {
    databaseAccountOfferType: 'Standard'
    locations: [
      {
        locationName: location
        failoverPriority: 0
      }
    ]
    capabilities: [
      {
        name: 'EnableTable'
      }
    ]
  }
}

resource birdVoiceDb 'Microsoft.DocumentDb/databaseAccounts/sqlDatabases@2023-05-01' = {
  parent: cosmosDb
  name: 'birdvoice'
}

resource audioFilesContainer 'Microsoft.DocumentDb/databaseAccounts/sqlDatabases/containers@2023-05-01' = {
  parent: birdVoiceDb
  name: 'audioFiles'
  properties: {
    partitionKey: {
      paths: ['/Id']
      kind: 'Hash'
    }
  }
}

resource audioFileAnalysisContainer 'Microsoft.DocumentDb/databaseAccounts/sqlDatabases/containers@2023-05-01' = {
  parent: birdVoiceDb
  name: 'audioFileAnalysis'
  properties: {
    partitionKey: {
      paths: ['/FileName']
      kind: 'Hash'
    }
  }
}

output blobStorageConnectionString string = 'DefaultEndpointsProtocol=https;AccountName=${birdVoices.name};AccountKey=${listKeys(birdVoices.id, '2023-05-01')[0].value};EndpointSuffix=core.windows.net'
output cosmosDbConnectionString string = 'AccountEndpoint=https://${cosmosDb.name}.documents.azure.com:443/;AccountKey=${listKeys(cosmosDb.id, '2023-05-01')[0].primaryMasterKey}'
