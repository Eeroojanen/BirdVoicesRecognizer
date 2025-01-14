using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using System.Linq;

public static class UploadVoiceAudio
{
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

        var validExtensions = new[] { ".wav", ".mp3" };
        var fileExtension = Path.GetExtension(file.FileName).ToLower();
        if (Array.IndexOf(validExtensions, fileExtension) == -1)
        {
            return new BadRequestObjectResult("Invalid file only .wav and .mp3 are allowed.");
        }

        var filePath = Path.Combine("C:\\Users\\Ertsi\\source\\repos\\BirdVoiceRecognizer\\BirdVoiceRecognizer\\Models", file.FileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        log.LogInformation("File uploaded and saved locally.");

        return new OkObjectResult("File uploaded successfully.");
    }

}
