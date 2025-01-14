using Azure.Storage.Blobs;
using System.IO;
using System.Threading.Tasks;

public static class BlobStorageService
{
    private static readonly string BlobConnectionString = "<BLOB_CONNECTION_STRING>";
    private static readonly string ContainerName = "audiofiles";

    public static async Task UploadFileToBlobAsync(string filePath)
    {
        var blobServiceClient = new BlobServiceClient(BlobConnectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
        var blobClient = containerClient.GetBlobClient(Path.GetFileName(filePath));

        using (var fileStream = File.OpenRead(filePath))
        {
            await blobClient.UploadAsync(fileStream);
        }
    }
}
