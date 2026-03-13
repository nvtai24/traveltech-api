using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using TravelTechApi.Common.Settings;

namespace TravelTechApi.Services.File
{
    public class S3Service : IFileService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly AwsSettings _settings;

        public S3Service(IOptions<AwsSettings> settings)
        {
            _settings = settings.Value;
            var config = new AmazonS3Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(_settings.Region)
            };
            var credentials = new Amazon.Runtime.BasicAWSCredentials(_settings.AccessKey, _settings.SecretKey);
            _s3Client = new AmazonS3Client(credentials, config);
        }

        public async Task<string> UploadImageAsync(Microsoft.AspNetCore.Http.IFormFile file, string folder = "general")
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty or null");

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            return await UploadImageAsync(file.OpenReadStream(), fileName, folder);
        }

        public async Task<string> UploadImageAsync(Stream stream, string fileName, string folder = "general")
        {
            var key = string.IsNullOrEmpty(folder) ? fileName : $"{folder}/{fileName}";

            var putRequest = new PutObjectRequest
            {
                BucketName = _settings.BucketName,
                Key = key,
                InputStream = stream,
                ContentType = GetContentType(fileName)
            };

            await _s3Client.PutObjectAsync(putRequest);

            // Construct the public URL
            return $"https://{_settings.BucketName}.s3.{_settings.Region}.amazonaws.com/{key}";
        }

        public async Task<bool> DeleteImageAsync(string fileKey)
        {
            if (string.IsNullOrEmpty(fileKey)) return false;

            try
            {
                var deleteRequest = new DeleteObjectRequest
                {
                    BucketName = _settings.BucketName,
                    Key = fileKey
                };

                var response = await _s3Client.DeleteObjectAsync(deleteRequest);
                return response.HttpStatusCode == System.Net.HttpStatusCode.NoContent || response.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<string>> DeleteMultipleImagesAsync(List<string> fileKeys)
        {
            var deletedKeys = new List<string>();
            foreach (var key in fileKeys)
            {
                var success = await DeleteImageAsync(key);
                if (success)
                {
                    deletedKeys.Add(key);
                }
            }
            return deletedKeys;
        }

        public async Task<List<string>> UploadMultipleImagesAsync(IEnumerable<Microsoft.AspNetCore.Http.IFormFile> files, string folder = "general", int maxConcurrency = 10)
        {
            var urls = new List<string>();
            var tasks = new List<Task<string>>();
            
            var throttler = new SemaphoreSlim(maxConcurrency);

            foreach (var file in files)
            {
                await throttler.WaitAsync();
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        return await UploadImageAsync(file, folder);
                    }
                    finally
                    {
                        throttler.Release();
                    }
                }));
            }

            var results = await Task.WhenAll(tasks);
            urls.AddRange(results);
            
            return urls;
        }

        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream",
            };
        }
    }
}
