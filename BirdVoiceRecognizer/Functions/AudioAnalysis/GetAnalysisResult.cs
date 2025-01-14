using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using BirdVoiceRecognizer.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public static class GetAnalysisResult
{
    [FunctionName("GetAnalysisResult")]
    public static async Task<IActionResult> GetAnalysis(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "result/{fileName}")] HttpRequest req,
        string fileName,
        ILogger log)
    {
        log.LogInformation($"Fetching result for file {fileName}...");

        var result = await CosmoDBService.GetAudioFileAsync(fileName);
        if (result == null)
        {
            return new NotFoundObjectResult("No analysis result found.");
        }

        return new OkObjectResult($"Analysis result for {fileName}: {result.AnalysisResult}");
    }
}
