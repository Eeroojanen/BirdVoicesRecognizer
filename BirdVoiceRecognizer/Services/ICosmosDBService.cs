using BirdVoiceRecognizer.Models;
using System.Threading.Tasks;

public interface ICosmosDBService
{
    Task<AudioFile> GetAudioFileAsync(string fileName);
    Task SaveAudioFileAsync(AudioFile audioFile);
    Task<AudioFileAnalysis> GetAnalysisResultAsync(string fileName);
    Task SaveAnalysisResultAsync(AudioFileAnalysis analysisResult);
}
