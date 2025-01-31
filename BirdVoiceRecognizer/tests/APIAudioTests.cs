using System.IO;
using System.Text;
using System.Threading.Tasks;
using BirdVoiceRecognizer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
namespace BirdVoiceRecognizer.tests
{
    public class APIAudioTests
    {
        private readonly Mock<IBlobStorageService> _mockBlobStorageService;
        private readonly Mock<ICosmosDBService> _mockCosmosDBService;
        private readonly Mock<ILogger<APIAudio>> _mockLogger;
        private readonly APIAudio _apiAudio;

        public APIAudioTests()
        {
            _mockBlobStorageService = new Mock<IBlobStorageService>();
            _mockCosmosDBService = new Mock<ICosmosDBService>();
            _mockLogger = new Mock<ILogger<APIAudio>>();
            _apiAudio = new APIAudio(_mockBlobStorageService.Object, _mockCosmosDBService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task UploadAudioFile_ValidFile_ReturnsOk()
        {
            var mockFile = new FormFile(
                new MemoryStream(Encoding.UTF8.GetBytes("Fake MP3 Content")),
                0,
                20,
                "file",
                "test.mp3"
            )
            {
                Headers = new HeaderDictionary(),
                ContentType = "audio/mpeg"
            };

            var formCollection = new FormCollection(null, new FormFileCollection { mockFile });
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Form = formCollection;

            var mockRequest = httpContext.Request;

            _mockBlobStorageService
                .Setup(service => service.UploadFileToBlobAsync(It.IsAny<string>(), It.IsAny<Stream>()))
                .ReturnsAsync("https://example.com/test.mp3");

            _mockCosmosDBService
                .Setup(service => service.SaveAudioFileAsync(It.IsAny<AudioFile>()))
                .Returns(Task.CompletedTask);

            _mockCosmosDBService
                .Setup(service => service.SaveAnalysisResultAsync(It.IsAny<AudioFileAnalysis>()))
                .Returns(Task.CompletedTask);

            var result = await _apiAudio.UploadAudioFile(mockRequest);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("File uploaded successfully and analysis saved", okResult.Value);

            _mockBlobStorageService.Verify(
                service => service.UploadFileToBlobAsync(It.IsAny<string>(), It.IsAny<Stream>()),
                Times.Once
            );

            _mockCosmosDBService.Verify(
                service => service.SaveAudioFileAsync(It.IsAny<AudioFile>()),
                Times.Once
            );

            _mockCosmosDBService.Verify(
                service => service.SaveAnalysisResultAsync(It.IsAny<AudioFileAnalysis>()),
                Times.Once
            );
        }

        [Fact]
        public async Task GetAudioFileAnalysis_ValidFileName_ReturnsOk()
        {
            var mockAnalysisResult = new AudioFileAnalysis
            {
                id = Guid.NewGuid().ToString(),
                FileName = "sound.mp3",
                AnalysisResult = "Sparrow",
                ConfidenceScore = 0.80,
                AnalysisTime = DateTime.UtcNow
            };

            _mockCosmosDBService
                .Setup(service => service.GetAnalysisResultAsync("sound.mp3"))
                .ReturnsAsync(mockAnalysisResult);

            var result = await _apiAudio.GetAudioFileAnalysis(new DefaultHttpContext().Request, "sound.mp3");

            var okResult = Assert.IsType<OkObjectResult>(result);
            var audioFileAnalysis = Assert.IsType<AudioFileAnalysis>(okResult.Value);

            Assert.Equal("sound.mp3", audioFileAnalysis.FileName);
            Assert.Equal("Sparrow", audioFileAnalysis.AnalysisResult);
            Assert.Equal(0.80, audioFileAnalysis.ConfidenceScore);

            _mockCosmosDBService.Verify(
                service => service.GetAnalysisResultAsync("sound.mp3"),
                Times.Once
            );
        }

        [Fact]
        public async Task GetAudioFileAnalysis_InvalidFileName_ReturnsNotFound()
        {
            _mockCosmosDBService
                .Setup(service => service.GetAnalysisResultAsync("nonexistent.mp3"))
                .ReturnsAsync((AudioFileAnalysis)null);

            var result = await _apiAudio.GetAudioFileAnalysis(new DefaultHttpContext().Request, "nonexistent.mp3");

            Assert.IsType<NotFoundResult>(result);

            _mockCosmosDBService.Verify(
                service => service.GetAnalysisResultAsync("nonexistent.mp3"),
                Times.Once
            );
        }

    }
}
