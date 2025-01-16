param location string = 'East US'
param resourceGroupName string = 'interviewreport-dev-eastus'
param openAiResourceName string = 'openai-resource'
param openAiSku string = 'S1'  // SKU for the OpenAI service (can be adjusted based on your needs)

targetScope = 'subscription'

resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: resourceGroupName
  location: location
}

resource openAi 'Microsoft.CognitiveServices/accounts@2021-10-01' = {
    name: openAiResourceName
    location: location
    kind: 'OpenAI'
    sku: {
      name: openAiSku
    }
    properties: {
      apiProperties: {
        apiKey: '<your-api-key>'  // Optional, depending on your setup
      }
    }
  }
  
  output openAiResourceId string = openAi.id
  output openAiEndpoint string = openAi.properties.endpoint
