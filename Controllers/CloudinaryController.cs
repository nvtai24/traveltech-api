using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelTechApi.Common.Constants;
using TravelTechApi.Common.Extensions;
using TravelTechApi.Services.Cloudinary;

namespace TravelTechApi.Controllers
{
    /// <summary>
    /// Test controller for Cloudinary image upload functionality
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CloudinaryController : ControllerBase
    {
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ILogger<CloudinaryController> _logger;

        public CloudinaryController(
            ICloudinaryService cloudinaryService,
            ILogger<CloudinaryController> logger)
        {
            _cloudinaryService = cloudinaryService;
            _logger = logger;
        }

        /// <summary>
        /// Upload an image to Cloudinary
        /// </summary>
        /// <param name="file">Image file to upload</param>
        /// <param name="folder">Optional folder name (default: "test")</param>
        /// <returns>Upload result with image details</returns>
        [Authorize(Roles = AppRoles.Admin + "," + AppRoles.User + "," + AppRoles.Moderator)]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file, [FromQuery] string folder = "general")
        {
            try
            {
                _logger.LogInformation("Uploading image: {FileName} to folder: {Folder}", file.FileName, folder);

                var result = await _cloudinaryService.UploadImageAsync(file, folder);

                _logger.LogInformation("Image uploaded successfully. PublicId: {PublicId}", result.PublicId);

                return this.Success(result, "Image uploaded successfully");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Upload validation failed: {Message}", ex.Message);
                return this.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image to Cloudinary");
                return this.InternalServerError("Failed to upload image");
            }
        }

        /// <summary>
        /// Get transformed image URL
        /// </summary>
        /// <param name="publicId">Public ID of the image</param>
        /// <param name="width">Optional width for transformation</param>
        /// <param name="height">Optional height for transformation</param>
        /// <returns>Transformed image URL</returns>
        [HttpGet("url/{**publicId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetImageUrl(string publicId, [FromQuery] int? width, [FromQuery] int? height)
        {
            try
            {
                _logger.LogInformation("Getting URL for publicId: {PublicId}, width: {Width}, height: {Height}",
                    publicId, width, height);

                var url = await _cloudinaryService.GetImageUrlAsync(publicId, width, height);

                return this.Success(new { url }, "Image URL retrieved successfully");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Get URL validation failed: {Message}", ex.Message);
                return this.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting image URL from Cloudinary");
                return this.InternalServerError("Failed to get image URL");
            }
        }

        /// <summary>
        /// Delete an image from Cloudinary
        /// </summary>
        /// <param name="publicId">Public ID of the image to delete (URL encoded)</param>
        /// <returns>Success or failure result</returns>
        // [HttpDelete("{**publicId}")]
        // public async Task<IActionResult> DeleteImage(string publicId)
        // {
        //     try
        //     {
        //         // Decode the publicId to handle folder paths (e.g., test%2Fimage -> test/image)
        //         var decodedPublicId = Uri.UnescapeDataString(publicId);

        //         _logger.LogInformation("Deleting image with publicId: {PublicId} (decoded: {DecodedPublicId})",
        //             publicId, decodedPublicId);

        //         var success = await _cloudinaryService.DeleteImageAsync(decodedPublicId);

        //         if (success)
        //         {
        //             _logger.LogInformation("Image deleted successfully. PublicId: {PublicId}", decodedPublicId);
        //             return this.Success("Image deleted successfully");
        //         }
        //         else
        //         {
        //             _logger.LogWarning("Failed to delete image. PublicId: {PublicId}", decodedPublicId);
        //             return this.NotFound("Image not found or already deleted");
        //         }
        //     }
        //     catch (ArgumentException ex)
        //     {
        //         _logger.LogWarning("Delete validation failed: {Message}", ex.Message);
        //         return this.BadRequest(ex.Message);
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error deleting image from Cloudinary");
        //         return this.InternalServerError("Failed to delete image");
        //     }
        // }

        /// <summary>
        /// Upload multiple images to Cloudinary in parallel (optimized for bulk uploads)
        /// </summary>
        /// <param name="files">List of image files to upload</param>
        /// <param name="folder">Optional folder name (default: "test")</param>
        /// <param name="maxConcurrency">Maximum number of concurrent uploads (default: 10)</param>
        /// <returns>List of upload results with success status for each file</returns>
        [HttpPost("upload-multiple")]
        public async Task<IActionResult> UploadMultipleImages(
            List<IFormFile> files,
            [FromQuery] string folder = "test",
            [FromQuery] int maxConcurrency = 10)
        {
            try
            {
                if (files == null || files.Count == 0)
                {
                    return this.BadRequest("No files provided");
                }

                _logger.LogInformation("Uploading {Count} images to folder: {Folder} with max concurrency: {MaxConcurrency}",
                    files.Count, folder, maxConcurrency);

                // Use the new parallel upload method
                var results = await _cloudinaryService.UploadMultipleImagesAsync(files, folder, maxConcurrency);

                var successCount = results.Count(r => r.IsSuccess);
                var failureCount = results.Count(r => !r.IsSuccess);

                _logger.LogInformation("Uploaded {SuccessCount}/{TotalCount} images successfully",
                    successCount, files.Count);

                return this.Success(results,
                    $"Uploaded {successCount}/{files.Count} images successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading multiple images to Cloudinary");
                return this.InternalServerError("Failed to upload images");
            }
        }
    }
}
