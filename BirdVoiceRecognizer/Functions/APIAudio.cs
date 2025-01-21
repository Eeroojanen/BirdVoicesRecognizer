using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using BirdVoiceRecognizer.Models;

public class APIAudio
{
    private readonly IBlobStorageService _blobStorageService;
    private readonly ICosmosDBService _cosmosDBService;

    public APIAudio(IBlobStorageService blobStorageService, ICosmosDBService cosmosDBService)
    {
        _blobStorageService = blobStorageService;
        _cosmosDBService = cosmosDBService;
    }

    [FunctionName("UploadAudioFile")]
    public async Task<IActionResult> UploadAudioFile(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "upload")] HttpRequest req)
    {
        var file = req.Form.Files["file"];
        if (file == null || file.Length == 0)
        {
            return new BadRequestObjectResult("No file uploaded.");
        }

        string blobUrl;
        using (var stream = file.OpenReadStream())
        {
            blobUrl = await _blobStorageService.UploadFileToBlobAsync(file.FileName, stream);
        }

        await SaveMetadata(new AudioFile
        {
            id = Guid.NewGuid().ToString(),
            FileName = file.FileName,
            ContentType = file.ContentType,
            FileSize = file.Length,
            UploadTime = DateTime.UtcNow,
            BlobUrl = blobUrl
        });

        return new OkObjectResult("File uploaded succesfully");
    }

    [FunctionName("GetAudioFileMetadata")]
    public async Task<IActionResult> GetAudioFileMetadata(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "audio/{fileName}")] HttpRequest req,
        string fileName)
    {
        var audioFile = await _cosmosDBService.GetAudioFileAsync(fileName);
        if (audioFile == null)
        {
            return new NotFoundResult();
        }

        return new OkObjectResult(audioFile);
    }

    public async Task SaveMetadata(AudioFile audioFile)
    {
       await _cosmosDBService.SaveAudioFileAsync(audioFile);
    }
}
