using System;
using Azure.Storage.Blobs;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(BirdVoiceRecognizer.Startup))]

namespace BirdVoiceRecognizer
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = builder.GetContext().Configuration;

            var blobConnectionString = config["BlobConnectionString"];
            if (string.IsNullOrEmpty(blobConnectionString))
            {
                throw new InvalidOperationException("BlobConnectionString is missing in configuration.");
            }

            var cosmosDbConnectionString = config["CosmosDbConnectionString"];
            if (string.IsNullOrEmpty(cosmosDbConnectionString))
            {
                throw new InvalidOperationException("CosmosDbConnectionString is missing in configuration.");
            }

            builder.Services.AddSingleton<IConfiguration>(config);

            builder.Services.AddSingleton(new BlobServiceClient(blobConnectionString));

            builder.Services.AddSingleton<IBlobStorageService, BlobStorageService>(sp =>
            {
                var blobServiceClient = sp.GetRequiredService<BlobServiceClient>();
                var containerName = "audiofiles";
                return new BlobStorageService(blobServiceClient, containerName);
            });

            builder.Services.AddSingleton(new CosmosClient(cosmosDbConnectionString));

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
