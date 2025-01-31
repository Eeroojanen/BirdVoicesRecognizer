using BirdVoiceRecognizer.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents;
using System;
using System.Linq;
using System.Threading.Tasks;
using PartitionKey = Microsoft.Azure.Cosmos.PartitionKey;
using QueryDefinition = Microsoft.Azure.Cosmos.QueryDefinition;

public class CosmosDBService : ICosmosDBService
{
    private readonly CosmosClient _cosmosClient;
    private readonly Container _audioFileContainer;
    private readonly Container _audioFileAnalysisContainer;

    public CosmosDBService(CosmosClient cosmosClient, string databaseName, string audioFileContainerName, string audioFileAnalysisContainerName)
    {
        _cosmosClient = cosmosClient;
        _audioFileContainer = _cosmosClient.GetContainer(databaseName, audioFileContainerName);
        _audioFileAnalysisContainer = _cosmosClient.GetContainer(databaseName, audioFileAnalysisContainerName);
    }


    public async Task<AudioFile> GetAudioFileAsync(string fileName)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.FileName = @fileName")
            .WithParameter("@fileName", fileName);

        var iterator = _audioFileContainer.GetItemQueryIterator<AudioFile>(query);
        var resultSet = await iterator.ReadNextAsync();

        return resultSet.FirstOrDefault();
    }

    public async Task SaveAudioFileAsync(AudioFile audioFile)
    {
        await _audioFileContainer.CreateItemAsync(audioFile, new PartitionKey(audioFile.id));
    }

    public async Task<AudioFileAnalysis> GetAnalysisResultAsync(string fileName)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.FileName = @fileName")
            .WithParameter("@fileName", fileName);

        var iterator = _audioFileAnalysisContainer.GetItemQueryIterator<AudioFileAnalysis>(query);
        var resultSet = await iterator.ReadNextAsync();

        return resultSet.FirstOrDefault();
    }

    public async Task SaveAnalysisResultAsync(AudioFileAnalysis analysisResult)
    {
        await _audioFileAnalysisContainer.CreateItemAsync(analysisResult, new PartitionKey(analysisResult.FileName));
    }
}
