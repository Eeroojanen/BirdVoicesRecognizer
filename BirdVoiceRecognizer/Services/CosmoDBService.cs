using BirdVoiceRecognizer.Models;
using Microsoft.Azure.Cosmos;
using System;
using System.Linq;
using System.Threading.Tasks;

public static class CosmoDBService
{
    private static readonly string CosmosDbConnectionString = Environment.GetEnvironmentVariable("CosmosDbConnectionString");
    private static readonly CosmosClient _cosmosClient = new CosmosClient(CosmosDbConnectionString);
    private static readonly Container _container = _cosmosClient.GetContainer("birdvoice", "audioFiles");

    public static async Task<AudioFile> GetAudioFileAsync(string fileName)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.FileName = @fileName")
            .WithParameter("@fileName", fileName);

        var iterator = _container.GetItemQueryIterator<AudioFile>(query);
        var resultSet = await iterator.ReadNextAsync();

        return resultSet.FirstOrDefault();
    }

    public static async Task SaveAudioFileAsync(AudioFile audioFile)
    {
        await _container.CreateItemAsync(audioFile, new PartitionKey(audioFile.Id));
    }
}
