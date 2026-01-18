namespace TravelTechApi.DTOs.Cloudinary
{
    /// <summary>
    /// Result of a single file upload in a bulk upload operation
    /// </summary>
    public class CloudinaryBulkUploadResult
    {
        /// <summary>
        /// Original file name
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether the upload was successful
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Upload result details (null if upload failed)
        /// </summary>
        public CloudinaryUploadResult? Result { get; set; }

        /// <summary>
        /// Error message if upload failed (null if successful)
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
}
