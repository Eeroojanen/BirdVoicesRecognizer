using Azure.Storage.Blobs;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(BirdVoiceRecognizer.Startup))]

namespace BirdVoiceRecognizer
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton(sp =>
            {
                var blobConnectionString = Environment.GetEnvironmentVariable("BlobConnectionString");
                if (string.IsNullOrEmpty(blobConnectionString))
                {
                    throw new InvalidOperationException("BlobConnectionString is not set in environment variables.");
                }
                return new BlobServiceClient(blobConnectionString);
            });

            builder.Services.AddSingleton<IBlobStorageService, BlobStorageService>(sp =>
            {
                var blobServiceClient = sp.GetRequiredService<BlobServiceClient>();
                var containerName = "audiofiles";
                return new BlobStorageService(blobServiceClient, containerName);
            });

            builder.Services.AddSingleton(sp =>
            {
                var cosmosDbConnectionString = Environment.GetEnvironmentVariable("CosmosDbConnectionString");
                if (string.IsNullOrEmpty(cosmosDbConnectionString))
                {
                    throw new InvalidOperationException("CosmosDbConnectionString is not set in environment variables.");
                }
                return new CosmosClient(cosmosDbConnectionString);
            });

            builder.Services.AddSingleton<ICosmosDBService, CosmosDBService>(sp =>
            {
                var cosmosClient = sp.GetRequiredService<CosmosClient>();
                return new CosmosDBService(
                    cosmosClient,
                    "birdvoice",
                    "audioFiles",
                    "audioFileAnalysis"
                );
            });
        }
    }
}
