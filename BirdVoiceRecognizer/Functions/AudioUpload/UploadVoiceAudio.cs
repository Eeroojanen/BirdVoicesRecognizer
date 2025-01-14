using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BirdVoiceRecognizer.Functions.AudioRecognizer
{
    public static class UploadVoiceAudio
    {
        [FunctionName("UploadVoiceAudio")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "upload")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Processing audio upload...");

            try
            {
                var formFile = req.Form.Files["file"];
                if (formFile == null)
                {
                    return new BadRequestObjectResult(new { message = "No file uploaded." });
                }

                using (var memoryStream = new MemoryStream())
                {
                    await formFile.CopyToAsync(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    var audioData = memoryStream.ToArray();

                    log.LogInformation("File uploaded and stored in memory.");


                    return new OkObjectResult(new { message = "File uploaded successfully and processed." });
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error processing audio upload.");
                return new BadRequestObjectResult(new { message = "Failed to upload or process file." });
            }
        }
    }
}
