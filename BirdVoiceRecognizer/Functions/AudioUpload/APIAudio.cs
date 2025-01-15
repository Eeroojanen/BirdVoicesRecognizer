using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using BirdVoiceRecognizer.Models;

public static class APIAudio
{
    private static readonly string[] ValidExtensions = { ".wav", ".mp3" };

    [FunctionName("UploadAudio")]
    public static async Task<IActionResult> UploadAudio(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "upload")] HttpRequest req,
    ILogger log)
    {
        log.LogInformation("Processing upload request...");

        if (!req.Form.Files.Any())
        {
            return new BadRequestObjectResult("No file found in the request.");
        }

        var file = req.Form.Files[0];
        var fileExtension = Path.GetExtension(file.FileName).ToLower();

        if (!ValidExtensions.Contains(fileExtension))
        {
            return new BadRequestObjectResult("Invalid file type. Only .wav and .mp3 are allowed.");
        }

        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var blobUrl = await BlobStorageService.UploadFileToBlobAsync(file.FileName, memoryStream);

            var audioFile = new AudioFile
            {
                id = Guid.NewGuid().ToString(),
                fileName = file.FileName,
                content = null,
                blobUrl = blobUrl,
                uploadedAt = DateTime.UtcNow
            };

            await CosmoDBService.SaveAudioFileAsync(audioFile);

            var analysisResult = await AnalyzeAudioInternal(file.FileName, log);

            log.LogInformation($"Analysis result: {analysisResult.Message}");
        }

        log.LogInformation("File uploaded and analyzed successfully.");

        return new OkObjectResult("File uploaded, analyzed, and saved successfully.");
    }

    private static async Task<dynamic> AnalyzeAudioInternal(string fileName, ILogger log)
{
    log.LogInformation("Simulating analysis of uploaded file...");

    await Task.Delay(100);

    var staticResponse = new
    {
        AnalysisId = Guid.NewGuid().ToString(),
        Message = "Analysis completed.",
        Result = "Bird species: Sparrow, Confidence: 85%"
    };

    var analysisResult = new AudioFileAnalysis
    {
        fileName = fileName,
        analysisResult = staticResponse.Result,
        analysisMessage = staticResponse.Message
    };

    await CosmoDBService.SaveAnalysisResultAsync(analysisResult);

    log.LogInformation("Returning static analysis result.");
    return staticResponse;
}

    [FunctionName("GetAnalysisResult")]
    public static async Task<IActionResult> GetAnalysisResult(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "result/{fileName}")] HttpRequest req,
    string fileName,
    ILogger log)
{
    log.LogInformation($"Fetching result for file {fileName}...");

    var result = await CosmoDBService.GetAnalysisResultAsync(fileName);
    if (result == null)
    {
        return new NotFoundObjectResult($"No analysis result found for file: {fileName}");
    }

    return new OkObjectResult(new
    {
        FileName = fileName,
        AnalysisResult = result.analysisResult
    });
}

}
