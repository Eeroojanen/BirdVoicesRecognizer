@description('Provide a name for the storage account. Use only lowercase letters and numbers. The name must be unique across Azure.')
param birdVoiceStorage string

@description('Location of the resources.')
param location string

resource birdVoices 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: birdVoiceStorage
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}

resource blobServices 'Microsoft.Storage/storageAccounts/blobServices@2023-05-01' = {
  name: 'default'
  parent: birdVoices
}

resource containers 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-05-01' = {
  name: 'audiofiles'
  parent: blobServices
  properties: {
    publicAccess: 'None'
  }
}

output storageName string = birdVoices.name
output storageKey string = listKeys(birdVoices.id, '2023-05-01').keys[0].value
output containerName string = containers.name
