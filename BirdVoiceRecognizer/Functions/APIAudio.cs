using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using BirdVoiceRecognizer.Models;
using Microsoft.Extensions.Logging;

public class APIAudio
{
    private readonly IBlobStorageService _blobStorageService;
    private readonly ICosmosDBService _cosmosDBService;
    private readonly ILogger<APIAudio> _logger;

    public APIAudio(IBlobStorageService blobStorageService, ICosmosDBService cosmosDBService, ILogger<APIAudio> logger)
    {
        _blobStorageService = blobStorageService;
        _cosmosDBService = cosmosDBService;
        _logger = logger;
    }

    [FunctionName("UploadAudioFile")]
    public async Task<IActionResult> UploadAudioFile(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "upload")] HttpRequest req)
    {
        var file = req.Form.Files["file"];
        if (file == null || file.Length == 0)
        {
            return new BadRequestObjectResult("No file uploaded");
        }

        string blobUrl;
        try
        {
            using var stream = file.OpenReadStream();
            blobUrl = await _blobStorageService.UploadFileToBlobAsync(file.FileName, stream);

            var audioFile = CreateAudioFileInstance(file, blobUrl);
            await _cosmosDBService.SaveAudioFileAsync(audioFile);

            var audioAnalysis = CreateAudioFileAnalysisInstance(file.FileName);
            await _cosmosDBService.SaveAnalysisResultAsync(audioAnalysis);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing file upload: {FileName}", file.FileName);
            return new ObjectResult($"An error occurred: {ex.Message}") { StatusCode = 500 };
        }

        return new OkObjectResult("File uploaded successfully and analysis saved");
    }


    [FunctionName("GetAudioFileAnalysis")]
    public async Task<IActionResult> GetAudioFileAnalysis(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "audio/{fileName}")] HttpRequest req,
        string fileName)
    {
        var audioFile = await _cosmosDBService.GetAnalysisResultAsync(fileName);
        if (audioFile == null)
        {
            return new NotFoundResult();
        }

        return new OkObjectResult(audioFile);
    }

    private AudioFile CreateAudioFileInstance(IFormFile file, string blobUrl)
    {
        return new AudioFile
        {
            id = Guid.NewGuid().ToString(),
            FileName = file.FileName,
            ContentType = file.ContentType,
            FileSize = file.Length,
            UploadTime = DateTime.UtcNow,
            BlobUrl = blobUrl
        };
    }

    private AudioFileAnalysis CreateAudioFileAnalysisInstance(string fileName)
    {
        (string result, double confidence) = GetAnalysisResult(fileName);

        return new AudioFileAnalysis
        {
            id = Guid.NewGuid().ToString(),
            FileName = fileName,
            AnalysisResult = result,
            ConfidenceScore = confidence,
            AnalysisTime = DateTime.UtcNow
        };
    }

    private (string Result, double Confidence) GetAnalysisResult(string fileName)
    {
        return ("Sparrow", 0.80);
    }
}