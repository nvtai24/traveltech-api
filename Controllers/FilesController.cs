using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelTechApi.Common.Constants;
using TravelTechApi.Common.Extensions;
using TravelTechApi.Services.File;

namespace TravelTechApi.Controllers
{
    /// <summary>
    /// Controller for generic file upload functionality via S3
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly ILogger<FilesController> _logger;

        public FilesController(
            IFileService fileService,
            ILogger<FilesController> logger)
        {
            _fileService = fileService;
            _logger = logger;
        }

        /// <summary>
        /// Upload a single file
        /// </summary>
        [Authorize(Roles = AppRoles.Admin + "," + AppRoles.User + "," + AppRoles.Moderator)]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file, [FromQuery] string folder = "general")
        {
            try
            {
                _logger.LogInformation("Uploading file: {FileName} to folder: {Folder}", file.FileName, folder);
                var url = await _fileService.UploadImageAsync(file, folder);
                _logger.LogInformation("File uploaded successfully. Url: {Url}", url);
                return this.Success(new { url }, "File uploaded successfully");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Upload validation failed: {Message}", ex.Message);
                return this.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file");
                return this.InternalServerError("Failed to upload file");
            }
        }

        /// <summary>
        /// Upload multiple files in parallel
        /// </summary>
        [Authorize(Roles = AppRoles.Admin + "," + AppRoles.User + "," + AppRoles.Moderator)]
        [HttpPost("upload-multiple")]
        public async Task<IActionResult> UploadMultipleFiles(
            List<IFormFile> files,
            [FromQuery] string folder = "general",
            [FromQuery] int maxConcurrency = 10)
        {
            try
            {
                if (files == null || files.Count == 0)
                {
                    return this.BadRequest("No files provided");
                }

                _logger.LogInformation("Uploading {Count} files to folder: {Folder} with max concurrency: {MaxConcurrency}",
                    files.Count, folder, maxConcurrency);

                var urls = await _fileService.UploadMultipleImagesAsync(files, folder, maxConcurrency);

                _logger.LogInformation("Uploaded {SuccessCount} files successfully", urls.Count);

                return this.Success(urls, $"Uploaded {urls.Count}/{files.Count} files successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading multiple files");
                return this.InternalServerError("Failed to upload files");
            }
        }
    }
}
