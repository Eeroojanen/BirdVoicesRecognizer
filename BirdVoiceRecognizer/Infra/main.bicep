@minLength(3)
@maxLength(24)
@description('Provide a name for the storage account. Use only lowercase letters and numbers. The name must be unique across Azure.')
param birdVoiceStorage string = 'store${uniqueString(resourceGroup().id)}'

@description('Provide a name for the Cosmos DB account. This must be globally unique.')
param cosmosDbDatabase string = 'cosmosdb${uniqueString(resourceGroup().id)}'

@description('Container name for birdvoice metadata')
param audioFiles string = 'audioFiles'

@description('Container for audiofiles analysis')
param audioFilesAnalysis string = 'audioFilesAnalysis'

@description('Location of the resources.')
param location string = resourceGroup().location  

@description('Provide a name for the Key Vault. The name must be globally unique.')
param keyVaultName string = 'keyVault${uniqueString(resourceGroup().id)}'

@description('Location of the Key Vault resource.')
param keyVaultLocation string = resourceGroup().location

@description('Define the secret for Cosmos DB connection string.')
param cosmosDbConnectionStringSecret string

@description('Define the secret for Storage Account connection string.')
param storageAccountConnectionStringSecret string



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
  name: '${cosmosDbDatabase}/birdvoice'
  location: location
  properties: {
    resource: {
      id: 'birdvoice'
    }
  }
}

resource audioFilesContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2024-11-15' = {
  name: '${cosmosDbDatabase}/birdvoice/${audioFiles}'
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
  name: '${cosmosDbDatabase}/birdvoice/${audioFilesAnalysis}'
  location: location
  properties: {
    resource: {
      id: audioFilesAnalysis
      partitionKey: {
      paths: ['/FileName']
      kind: 'Hash'
      }
    }
  }
}

resource birdVoices 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: birdVoiceStorage
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}

resource blobServices 'Microsoft.Storage/storageAccounts/blobServices@2023-05-01' =  {
  name: 'default'
  parent: birdVoices
}

resource containers 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-05-01' = {
  name: 'storage-container'
  parent: blobServices
  properties: {
    publicAccess: 'None'
  }
}

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: keyVaultName
  location: keyVaultLocation
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    accessPolicies: []
    tenantId: subscription().tenantId
    enabledForDeployment: true
    enabledForDiskEncryption: true
    enabledForTemplateDeployment: true
  }
}

resource storageAccountConnectionStringSecretResource 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  name: '${keyVaultName}/storageAccountConnectionString'
  properties: {
    value: storageAccountConnectionStringSecret
  }
}


resource cosmosDbConnectionStringSecretResource 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  name: '${keyVaultName}/cosmosDbConnectionString'
  properties: {
    value: cosmosDbConnectionStringSecret
  }
}

output keyVaultUri string = keyVault.properties.vaultUri
