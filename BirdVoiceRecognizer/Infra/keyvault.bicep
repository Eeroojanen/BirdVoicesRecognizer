@description('Provide a name for the Key Vault. The name must be globally unique.')
param keyVaultName string

@description('Location of the Key Vault resource.')
param keyVaultLocation string

@description('Storage Account connection string.')
param storageAccountConnectionString string

@description('Cosmos DB connection string.')
param cosmosDbConnectionString string

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

resource storageSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: 'storageAccountConnectionString'
  properties: {
    value: storageAccountConnectionString
  }
}

resource cosmosDbSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: 'cosmosDbConnectionString'
  properties: {
    value: cosmosDbConnectionString
  }
}

output keyVaultUri string = keyVault.properties.vaultUri
output storageSecretName string = storageSecret.name 
