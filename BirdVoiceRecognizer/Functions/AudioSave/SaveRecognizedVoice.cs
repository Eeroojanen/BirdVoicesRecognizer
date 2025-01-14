using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BirdVoiceRecognizer.Functions.AudioSave
{
    public static class SaveRecognizedVoice
    {
        [FunctionName("SaveRecognizedVoice")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "save")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Processing recognized voice save request...");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                if (string.IsNullOrEmpty(requestBody))
                {
                    return new BadRequestObjectResult(new { message = "No data received." });
                }

                log.LogInformation("Received recognized voice data: " + requestBody);

                return new OkObjectResult(new { message = "Voice data saved successfully." });
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error processing voice data save request.");
                return new BadRequestObjectResult(new { message = "Failed to save voice data." });
            }
        }
    }
}
