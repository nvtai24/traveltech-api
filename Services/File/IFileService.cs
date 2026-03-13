namespace TravelTechApi.Services.File
{
    public interface IFileService
    {
        Task<string> UploadImageAsync(Microsoft.AspNetCore.Http.IFormFile file, string folder = "general");
        Task<string> UploadImageAsync(Stream stream, string fileName, string folder = "general");
        Task<bool> DeleteImageAsync(string fileKey);
        Task<List<string>> DeleteMultipleImagesAsync(List<string> fileKeys);
        Task<List<string>> UploadMultipleImagesAsync(IEnumerable<Microsoft.AspNetCore.Http.IFormFile> files, string folder = "general", int maxConcurrency = 10);
    }
}
