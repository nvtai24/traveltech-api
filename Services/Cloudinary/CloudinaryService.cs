using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using TravelTechApi.Common.Settings;

namespace TravelTechApi.Services.Cloudinary
{
    /// <summary>
    /// Implementation of Cloudinary service for image management
    /// </summary>
    public class CloudinaryService : ICloudinaryService
    {
        private readonly CloudinaryDotNet.Cloudinary _cloudinary;
        private readonly ILogger<CloudinaryService> _logger;
        private readonly CloudinarySettings _settings;

        // Allowed image extensions
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };

        // Max file size: 10MB
        private const long MaxFileSize = 10 * 1024 * 1024;

        public CloudinaryService(
            IOptions<CloudinarySettings> settings,
            ILogger<CloudinaryService> logger)
        {
            _settings = settings.Value;
            _logger = logger;

            // Initialize Cloudinary account
            var account = new Account(
                _settings.CloudName,
                _settings.ApiKey,
                _settings.ApiSecret
            );

            _cloudinary = new CloudinaryDotNet.Cloudinary(account);
        }

        /// <inheritdoc/>
        public async Task<CloudinaryUploadResult> UploadImageAsync(IFormFile file, string folder = "general")
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is empty or null", nameof(file));
            }

            // Validate file size
            if (file.Length > MaxFileSize)
            {
                throw new ArgumentException($"File size exceeds maximum allowed size of {MaxFileSize / 1024 / 1024}MB", nameof(file));
            }

            // Validate file extension
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
            {
                throw new ArgumentException($"File type '{extension}' is not allowed. Allowed types: {string.Join(", ", AllowedExtensions)}", nameof(file));
            }

            try
            {
                using var stream = file.OpenReadStream();
                return await UploadImageAsync(stream, file.FileName, folder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image to Cloudinary: {FileName}", file.FileName);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<CloudinaryUploadResult> UploadImageAsync(Stream stream, string fileName, string folder = "general")
        {
            if (stream == null || stream.Length == 0)
            {
                throw new ArgumentException("Stream is empty or null", nameof(stream));
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("File name cannot be empty", nameof(fileName));
            }

            try
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(fileName, stream),
                    Folder = folder,
                    UseFilename = true,
                    UniqueFilename = true,
                    Overwrite = false
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    _logger.LogError("Cloudinary upload failed with status code: {StatusCode}, Error: {Error}",
                        uploadResult.StatusCode, uploadResult.Error?.Message);
                    throw new Exception($"Cloudinary upload failed: {uploadResult.Error?.Message}");
                }

                _logger.LogInformation("Successfully uploaded image to Cloudinary: {PublicId}", uploadResult.PublicId);

                return new CloudinaryUploadResult
                {
                    PublicId = uploadResult.PublicId,
                    Url = uploadResult.Url.ToString(),
                    SecureUrl = uploadResult.SecureUrl.ToString(),
                    Width = uploadResult.Width,
                    Height = uploadResult.Height,
                    Format = uploadResult.Format,
                    Bytes = uploadResult.Bytes
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image to Cloudinary: {FileName}", fileName);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteImageAsync(string publicId)
        {
            if (string.IsNullOrWhiteSpace(publicId))
            {
                throw new ArgumentException("Public ID cannot be empty", nameof(publicId));
            }

            try
            {
                var deleteParams = new DeletionParams(publicId)
                {
                    ResourceType = ResourceType.Image,
                    Invalidate = true
                };
                var result = await _cloudinary.DestroyAsync(deleteParams);

                if (result.Result == "ok")
                {
                    _logger.LogInformation("Successfully deleted image from Cloudinary: {PublicId}", publicId);
                    return true;
                }

                _logger.LogWarning("Failed to delete image from Cloudinary: {PublicId}, Result: {Result}",
                    publicId, result.Result);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image from Cloudinary: {PublicId}", publicId);
                throw;
            }
        }

        /// <inheritdoc/>
        public Task<string> GetImageUrlAsync(string publicId, int? width = null, int? height = null)
        {
            if (string.IsNullOrWhiteSpace(publicId))
            {
                throw new ArgumentException("Public ID cannot be empty", nameof(publicId));
            }

            try
            {
                var transformation = new Transformation();

                if (width.HasValue)
                {
                    transformation = transformation.Width(width.Value);
                }

                if (height.HasValue)
                {
                    transformation = transformation.Height(height.Value);
                }

                // Apply crop mode if both dimensions are specified
                if (width.HasValue && height.HasValue)
                {
                    transformation = transformation.Crop("fill");
                }

                var url = _cloudinary.Api.UrlImgUp.Transform(transformation).BuildUrl(publicId);

                _logger.LogDebug("Generated Cloudinary URL for {PublicId}: {Url}", publicId, url);

                return Task.FromResult(url);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating Cloudinary URL for: {PublicId}", publicId);
                throw;
            }
        }
    }
}
