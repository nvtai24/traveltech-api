using TravelTechApi.Services.Cloudinary;

namespace TravelTechApi.Services
{
    /// <summary>
    /// Service for managing Cloudinary image uploads and operations
    /// </summary>
    public interface ICloudinaryService
    {
        /// <summary>
        /// Upload an image from IFormFile
        /// </summary>
        /// <param name="file">The image file to upload</param>
        /// <param name="folder">Optional folder path in Cloudinary (default: "general")</param>
        /// <returns>Upload result containing image details</returns>
        Task<CloudinaryUploadResult> UploadImageAsync(IFormFile file, string folder = "general");

        /// <summary>
        /// Upload an image from a stream
        /// </summary>
        /// <param name="stream">The image stream to upload</param>
        /// <param name="fileName">The file name for the upload</param>
        /// <param name="folder">Optional folder path in Cloudinary (default: "general")</param>
        /// <returns>Upload result containing image details</returns>
        Task<CloudinaryUploadResult> UploadImageAsync(Stream stream, string fileName, string folder = "general");

        /// <summary>
        /// Delete an image from Cloudinary
        /// </summary>
        /// <param name="publicId">The public ID of the image to delete</param>
        /// <returns>True if deletion was successful, false otherwise</returns>
        Task<bool> DeleteImageAsync(string publicId);

        /// <summary>
        /// Get a transformed image URL
        /// </summary>
        /// <param name="publicId">The public ID of the image</param>
        /// <param name="width">Optional width for transformation</param>
        /// <param name="height">Optional height for transformation</param>
        /// <returns>The transformed image URL</returns>
        Task<string> GetImageUrlAsync(string publicId, int? width = null, int? height = null);

        /// <summary>
        /// Upload multiple images in parallel for better performance
        /// </summary>
        /// <param name="files">Collection of files to upload</param>
        /// <param name="folder">Optional folder path in Cloudinary (default: "general")</param>
        /// <param name="maxConcurrency">Maximum number of concurrent uploads (default: 10, recommended by Cloudinary)</param>
        /// <returns>List of upload results with success status for each file</returns>
        Task<List<CloudinaryBulkUploadResult>> UploadMultipleImagesAsync(
            IEnumerable<IFormFile> files,
            string folder = "general",
            int maxConcurrency = 10);
    }
}
