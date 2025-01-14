using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using BirdVoiceRecognizer.Models;
using System;

public static class UploadAudio
{
    private static readonly string[] ValidExtensions = { ".wav", ".mp3" };

    [FunctionName("UploadAudio")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "upload")] HttpRequest req,
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

            var audioFile = new AudioFile
            {
                Id = Guid.NewGuid().ToString(),
                FileName = file.FileName,
                Content = memoryStream.ToArray(),
                UploadedAt = DateTime.UtcNow
            };

            await CosmoDBService.SaveAudioFileAsync(audioFile);
        }

        log.LogInformation("File uploaded successfully.");

        return new OkObjectResult("File uploaded successfully.");
    }
}
