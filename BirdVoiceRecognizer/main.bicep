@minLength(3)
@maxLength(24)
@description('Provide a name for the storage account. Use only lowercase letters and numbers. The name must be unique across Azure.')
param birdVoiceStorage string = 'store${uniqueString(resourceGroup().id)}'
param location string = resourceGroup().location

resource birdVoices 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: birdVoiceStorage
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'BlobStorage'
}
