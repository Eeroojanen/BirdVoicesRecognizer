using Azure.Storage.Blobs;
using System;
using System.IO;
using System.Threading.Tasks;

public static class BlobStorageService
{
    private static readonly string BlobConnectionString = Environment.GetEnvironmentVariable("BlobConnectionString");
    private static readonly string ContainerName = "audiofiles";

    public static async Task<string> UploadFileToBlobAsync(string fileName, Stream fileStream)
    {
        var blobServiceClient = new BlobServiceClient(BlobConnectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);

        await containerClient.CreateIfNotExistsAsync();

        var blobClient = containerClient.GetBlobClient(fileName);

        await blobClient.UploadAsync(fileStream, overwrite: true);

        return blobClient.Uri.ToString();
    }

}
