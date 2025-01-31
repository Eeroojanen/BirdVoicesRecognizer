using Azure.Storage.Blobs;
using System;
using System.IO;
using System.Threading.Tasks;

public class BlobStorageService : IBlobStorageService
{
    private readonly string _containerName;
    private readonly BlobServiceClient _blobServiceClient;

    public BlobStorageService(BlobServiceClient blobServiceClient, string containerName)
    {
        _blobServiceClient = blobServiceClient;
        _containerName = containerName;
    }

    public async Task<string> UploadFileToBlobAsync(string fileName, Stream fileStream){

        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

        await containerClient.CreateIfNotExistsAsync();

        var blobClient = containerClient.GetBlobClient(fileName);

        await blobClient.UploadAsync(fileStream, overwrite: true);

        return blobClient.Uri.ToString();
    }
}
