@description('Name of the Cosmos DB account. Must be globally unique.')
param cosmosDbDatabase string

@description('Container name for birdvoice metadata.')
param audioFiles string

@description('Container name for audio file analysis.')
param audioFileAnalysis string

@description('Location of the Cosmos DB resources.')
param location string

resource cosmosDb 'Microsoft.DocumentDB/databaseAccounts@2024-11-15' = {
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
  }
}

resource birdVoiceDb 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2024-11-15' = {
  parent: cosmosDb
  name: 'birdvoice'
  location: location
  properties: {
    resource: {
      id: 'birdvoice'
    }
  }
}

resource audioFilesContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2024-11-15' = {
  parent: birdVoiceDb
  name: audioFiles
  properties: {
    resource: {
      id: audioFiles
      partitionKey: {
        paths: ['/id']
        kind: 'Hash'
      }
    }
  }
}

resource audioFilesAnalysisContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2024-11-15' = {
  parent: birdVoiceDb
  name: audioFileAnalysis
  properties: {
    resource: {
      id: audioFileAnalysis
      partitionKey: {
        paths: ['/FileName']
        kind: 'Hash'
      }
    }
  }
}

output accountName string = cosmosDb.name
output accountKey string = listKeys(cosmosDb.id, '2024-11-15').primaryMasterKey
