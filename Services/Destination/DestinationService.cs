using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TravelTechApi.Common.Utils;
using TravelTechApi.Data;
using TravelTechApi.DTOs.Destination;
using TravelTechApi.Entities;
using TravelTechApi.Services.Cloudinary;

namespace TravelTechApi.Services.Destination
{
    /// <summary>
    /// Service implementation for Destination operations
    /// </summary>
    public class DestinationService : IDestinationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<DestinationService> _logger;
        private readonly ICloudinaryService _cloudinaryService;

        public DestinationService(
            ApplicationDbContext context,
            IMapper mapper,
            ILogger<DestinationService> logger,
            ICloudinaryService cloudinaryService)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<IEnumerable<DestinationResponse>> GetAllDestinationsAsync(int? regionId, int? locationId, string? keyword)
        {
            _logger.LogInformation("Getting destinations by region id: {RegionId}, location id: {LocationId}, keyword: {Keyword}", regionId, locationId, keyword);
            var destinations = await _context.Destinations
                .Include(d => d.Location)
                .ThenInclude(l => l.Region)
                .Include(d => d.Images)
                .OrderBy(d => d.Name)
                .ToListAsync();

            if (regionId.HasValue)
            {
                destinations = destinations.Where(d => d.Location.RegionId == regionId.Value).ToList();
            }

            if (locationId.HasValue)
            {
                destinations = destinations.Where(d => d.LocationId == locationId.Value).ToList();
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = TextUtils.RemoveDiacritics(keyword).Trim();
                destinations = destinations.Where(d => TextUtils.CheckContains(d.Name, keyword)
                 || TextUtils.CheckContains(d.Title, keyword)
                 || TextUtils.CheckContains(d.Description, keyword)
                 || TextUtils.CheckContains(d.History, keyword)
                 || d.Tags.Any(tag => TextUtils.CheckContains(tag, keyword))).ToList();
            }

            return _mapper.Map<IEnumerable<DestinationResponse>>(destinations);
        }

        public async Task<DestinationDetailsResponse?> GetDestinationByIdAsync(int id)
        {
            _logger.LogInformation("Getting destination by id: {DestinationId}", id);
            var destination = await _context.Destinations
                .Include(d => d.Location)
                .Include(d => d.Images)
                .Include(d => d.FAQs)
                .FirstOrDefaultAsync(d => d.Id == id);

            return destination == null ? null : _mapper.Map<DestinationDetailsResponse>(destination);
        }
        public async Task<IEnumerable<DestinationSharingResponse>> GetDestinationsSharingsAsync(int destinationId)
        {
            _logger.LogInformation("Getting destinations sharings by destination id: {DestinationId}", destinationId);
            var destinationSharings = await _context.DestinationSharings
                .Include(ds => ds.Images)
                .Include(ds => ds.User)
                .Where(ds => ds.DestinationId == destinationId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<DestinationSharingResponse>>(destinationSharings);
        }

        public async Task<DestinationSharingResponse> CreateDestinationSharingAsync(int destinationId, string userId, CreateDestinationSharingRequest dto)
        {
            _logger.LogInformation("Creating destination sharing for destination {DestinationId} by user {UserId}", destinationId, userId);

            // Validate destination exists
            var destination = await _context.Destinations.FindAsync(destinationId);
            if (destination == null)
            {
                throw new ArgumentException($"Destination with id {destinationId} not found");
            }

            // Create sharing entity
            var sharing = new DestinationSharing
            {
                DestinationId = destinationId,
                UserId = userId,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            // Upload images if provided
            var uploadedImages = new List<CloudinaryFileInfo>();
            if (dto.Images != null && dto.Images.Any())
            {
                _logger.LogInformation("Uploading {Count} images for sharing", dto.Images.Count);

                // Upload images in parallel using Cloudinary bulk upload
                var uploadResults = await _cloudinaryService.UploadMultipleImagesAsync(
                    dto.Images,
                    folder: $"destinations/{destinationId}/sharings",
                    maxConcurrency: 10
                );

                // Process successful uploads
                foreach (var result in uploadResults.Where(r => r.IsSuccess && r.Result != null))
                {
                    var imageInfo = new CloudinaryFileInfo
                    {
                        PublicId = result.Result!.PublicId,
                        Url = result.Result.SecureUrl,
                        SecureUrl = result.Result.SecureUrl,
                        Format = result.Result.Format,
                        ResourceType = result.Result.ResourceType,
                        CreatedAt = DateTime.UtcNow
                    };
                    uploadedImages.Add(imageInfo);
                }

                _logger.LogInformation("Successfully uploaded {SuccessCount}/{TotalCount} images",
                    uploadedImages.Count, dto.Images.Count);
            }

            // Add images to sharing
            sharing.Images = uploadedImages;

            // Save to database
            _context.DestinationSharings.Add(sharing);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Destination sharing created successfully with id {SharingId}", sharing.Id);

            // Load user info for response
            await _context.Entry(sharing).Reference(s => s.User).LoadAsync();

            return _mapper.Map<DestinationSharingResponse>(sharing);
        }

        public async Task<DestinationDetailsResponse> CreateDestinationAsync(CreateDestinationRequest dto)
        {
            _logger.LogInformation("Creating new destination: {Name}", dto.Name);

            // Validate location exists
            var location = await _context.Locations.FindAsync(dto.LocationId);
            if (location == null)
            {
                throw new ArgumentException($"Location with id {dto.LocationId} not found");
            }

            // Map DTO to entity using AutoMapper
            var destination = _mapper.Map<Entities.Destination>(dto);

            // Upload images if provided
            if (dto.Images != null && dto.Images.Any())
            {
                _logger.LogInformation("Uploading {Count} images for destination", dto.Images.Count);

                var uploadResults = await _cloudinaryService.UploadMultipleImagesAsync(
                    dto.Images,
                    folder: "destinations",
                    maxConcurrency: 10
                );

                // Process successful uploads
                var uploadedImages = uploadResults
                    .Where(r => r.IsSuccess && r.Result != null)
                    .Select(r => _mapper.Map<CloudinaryFileInfo>(r.Result))
                    .ToList();


                destination.Images = uploadedImages;

                _logger.LogInformation("Successfully uploaded {SuccessCount}/{TotalCount} images",
                    uploadedImages.Count, dto.Images.Count);
            }

            // Map FAQs if provided using AutoMapper
            if (dto.FAQs != null && dto.FAQs.Any())
            {
                destination.FAQs = _mapper.Map<List<FAQ>>(dto.FAQs);
                _logger.LogInformation("Added {Count} FAQs to destination", destination.FAQs.Count);
            }

            // Save to database
            _context.Destinations.Add(destination);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Destination created successfully with id {DestinationId}", destination.Id);

            // Load related data for response
            await _context.Entry(destination).Reference(d => d.Location).LoadAsync();
            await _context.Entry(destination).Collection(d => d.Images).LoadAsync();
            await _context.Entry(destination).Collection(d => d.FAQs).LoadAsync();

            return _mapper.Map<DestinationDetailsResponse>(destination);
        }

    }
}
