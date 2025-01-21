using System.IO;
using System.Threading.Tasks;

public interface IBlobStorageService
{
    Task<string> UploadFileToBlobAsync(string fileName, Stream fileStream);
}