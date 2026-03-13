using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TravelTechApi.Common.Utils;
using TravelTechApi.Data;
using TravelTechApi.DTOs.Common;
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
        private readonly StackExchange.Redis.IConnectionMultiplexer _redis;

        public DestinationService(
            ApplicationDbContext context,
            IMapper mapper,
            ILogger<DestinationService> logger,
            ICloudinaryService cloudinaryService,
            StackExchange.Redis.IConnectionMultiplexer redis)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _cloudinaryService = cloudinaryService;
            _redis = redis;
        }

        public async Task<IEnumerable<DestinationResponse>> GetAllDestinationsAsync(int? regionId, int? locationId, string? keyword)
        {
            _logger.LogInformation("Getting destinations by region id: {RegionId}, location id: {LocationId}, keyword: {Keyword}", regionId, locationId, keyword);

            var db = _redis.GetDatabase();
            var version = await GetCacheVersion();
            string cacheKey = $"destinations:all:v{version}:{regionId}:{locationId}:{keyword}";

            // Try get from cache
            var cachedData = await db.StringGetAsync(cacheKey);
            if (!cachedData.IsNullOrEmpty)
            {
                _logger.LogInformation("Cache hit for key: {Key}", cacheKey);
                return System.Text.Json.JsonSerializer.Deserialize<IEnumerable<DestinationResponse>>(cachedData!)!;
            }

            var query = _context.Destinations
                .Include(d => d.Location)
                .ThenInclude(l => l.Region)
                .Include(d => d.Images)
                .Where(d => d.IsVisible) // Enforce visibility for public API
                .OrderBy(d => d.Name)
                .AsQueryable();

            if (regionId.HasValue)
            {
                query = query.Where(d => d.Location.RegionId == regionId.Value);
            }

            if (locationId.HasValue)
            {
                query = query.Where(d => d.LocationId == locationId.Value);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(d => EF.Functions.ILike(EF.Functions.Unaccent(d.Name), "%" + EF.Functions.Unaccent(keyword) + "%")
                 || EF.Functions.ILike(EF.Functions.Unaccent(d.Title), "%" + EF.Functions.Unaccent(keyword) + "%")
                 || EF.Functions.ILike(EF.Functions.Unaccent(d.Description), "%" + EF.Functions.Unaccent(keyword) + "%")
                 || EF.Functions.ILike(EF.Functions.Unaccent(d.History), "%" + EF.Functions.Unaccent(keyword) + "%")
                 || d.Tags.Any(tag => EF.Functions.ILike(EF.Functions.Unaccent(tag), "%" + EF.Functions.Unaccent(keyword) + "%")));
            }

            var destinations = await query.ToListAsync();
            var result = _mapper.Map<IEnumerable<DestinationResponse>>(destinations);

            // Set cache
            await db.StringSetAsync(cacheKey,
                System.Text.Json.JsonSerializer.Serialize(result),
                TimeSpan.FromMinutes(30));

            return result;
        }

        private async Task InvalidateDestinationsCache()
        {
            var db = _redis.GetDatabase();
            // Invalidate all destination keys using a pattern requires SCAN, which is expensive.
            // For now, simpler approach: Use a known set or just accept expiration.
            // Better approach for production: Store dependencies or use tagging if supported (not out of box in Redis).
            // Or simplest: Just delete the most common keys if predictable, or use a shorter cache time.

            // Alternative: Find keys matching pattern. Note: Keys usually blocks, SCAN avoids blocking but needs loop.
            // Since we can't easily retrieve all keys to delete, we'll rely on short expiration (30m) 
            // OR if we want to be proactive, we can delete the 'all items' cache if that's the main one.

            // But wait, the key depends on filters. 
            // A common strategy without advanced tagging:
            // 1. Maintain a "version" key for destinations.
            // 2. Append version to all destination cache keys.
            // 3. Increment version on any update.
            // Let's implement that? It's robust.

            // Actually, for this specific request, let's keep it simple first.
            // If the user didn't ask for a complex versioning system, maybe just basic caching.
            // But invalidation is key.
            // Let's increment a 'destinations:version' key.

            await db.StringIncrementAsync("destinations:version");
        }

        // Helper to get current version
        private async Task<string> GetCacheVersion()
        {
            var db = _redis.GetDatabase();
            var val = await db.StringGetAsync("destinations:version");
            return val.HasValue ? val.ToString() : "0";
        }

        public async Task<PagedResult<DestinationAdminResponse>> GetAllDestinationsAdminAsync(int page, int pageSize, string? keyword)
        {
            _logger.LogInformation("Getting all destinations for admin - page {Page}, keyword {Keyword}", page, keyword);

            var query = _context.Destinations
                .Include(d => d.Location)
                .ThenInclude(l => l.Region)
                .Include(d => d.Images)
                .OrderByDescending(d => d.Id) // Newest first for admin
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(d => EF.Functions.ILike(EF.Functions.Unaccent(d.Name), "%" + EF.Functions.Unaccent(keyword) + "%")
                 || EF.Functions.ILike(EF.Functions.Unaccent(d.Title), "%" + EF.Functions.Unaccent(keyword) + "%")
                 || EF.Functions.ILike(EF.Functions.Unaccent(d.Description), "%" + EF.Functions.Unaccent(keyword) + "%")
                 || EF.Functions.ILike(EF.Functions.Unaccent(d.History), "%" + EF.Functions.Unaccent(keyword) + "%")
                 || d.Tags.Any(tag => EF.Functions.ILike(EF.Functions.Unaccent(tag), "%" + EF.Functions.Unaccent(keyword) + "%")));
            }


            var totalCount = await query.CountAsync();

            var destinations = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = _mapper.Map<IEnumerable<DestinationAdminResponse>>(destinations);

            return PagedResult<DestinationAdminResponse>.Create(dtos, totalCount, page, pageSize);
        }

        public async Task<DestinationDetailsAdminResponse?> GetDestinationByIdAdminAsync(int id)
        {
            _logger.LogInformation("Getting destination by id for admin: {DestinationId}", id);
            var destination = await _context.Destinations
                .Include(d => d.Location)
                .Include(d => d.Images)
                .Include(d => d.FAQs)
                .FirstOrDefaultAsync(d => d.Id == id);

            return destination == null ? null : _mapper.Map<DestinationDetailsAdminResponse>(destination);
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
            var uploadedImages = new List<string>();
            if (dto.Images != null && dto.Images.Any())
            {
                _logger.LogInformation("Uploading {Count} images for sharing", dto.Images.Count);

                // Upload images in parallel using Cloudinary bulk upload
                var uploadResults = await _cloudinaryService.UploadMultipleImagesAsync(
                    dto.Images,
                    folder: $"destinations/{destinationId}/sharings",
                    maxConcurrency: 10
                );

                foreach (var result in uploadResults.Where(r => r.IsSuccess && r.Result != null))
                {
                    uploadedImages.Add(result.Result!.SecureUrl);
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
                    .Select(r => r.Result.SecureUrl)
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

            await InvalidateDestinationsCache();

            return _mapper.Map<DestinationDetailsResponse>(destination);
        }

        public async Task<DestinationDetailsResponse> UpdateDestinationAsync(int id, UpdateDestinationRequest dto)
        {
            _logger.LogInformation("Updating destination with id: {DestinationId}", id);

            var destination = await _context.Destinations
                .Include(d => d.Location)
                .Include(d => d.Images)
                .Include(d => d.FAQs)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (destination == null)
            {
                _logger.LogWarning("Destination not found: {DestinationId}", id);
                throw new KeyNotFoundException($"Destination with id {id} not found");
            }

            // Update fields
            destination.Name = dto.Name;
            destination.Title = dto.Title;
            destination.Description = dto.Description;
            destination.History = dto.History;
            destination.Lat = dto.Lat;
            destination.Lon = dto.Lon;
            destination.VideoUrl = dto.VideoUrl;
            destination.Tags = dto.Tags;
            destination.LocationId = dto.LocationId;
            destination.IsVisible = dto.IsVisible;



            // Validate location if changed
            if (destination.LocationId != dto.LocationId)
            {
                var location = await _context.Locations.FindAsync(dto.LocationId);
                if (location == null)
                {
                    throw new ArgumentException($"Location with id {dto.LocationId} not found");
                }
            }

            // Handle existing images (sync logic)
            if (dto.ExistingImageUrls != null)
            {
                // Identify images to remove (in DB but not in kept list)
                var imagesToRemove = destination.Images
                    .Where(imgUrl => !dto.ExistingImageUrls.Contains(imgUrl))
                    .ToList();

                if (imagesToRemove.Any())
                {
                    _logger.LogInformation("Removing {Count} images from destination {DestinationId}", imagesToRemove.Count, id);

                    // Collect PublicIds for Cloudinary deletion
                    var publicIdsToDelete = imagesToRemove
                        .Select(imgUrl => ExtractPublicIdFromUrl(imgUrl))
                        .Where(id => !string.IsNullOrEmpty(id))
                        .ToList();

                    if (publicIdsToDelete.Any())
                    {
                        await _cloudinaryService.DeleteMultipleImagesAsync(publicIdsToDelete);
                    }

                    // Remove from database/collection
                    foreach (var imgUrl in imagesToRemove)
                    {
                        destination.Images.Remove(imgUrl);
                    }
                }
            }
            else
            {
                // If ExistingImageUrls is null, consider if we should keep all or delete all? 
                // Usually matching frontend behavior: if they send null, maybe they didn't touch images? 
                // Or if they send empty list, they want to delete all?
                // For safety, let's assume if null, we don't delete anything (keep existing).
                // If user wants to delete all, they should send valid empty list.
            }
            if (dto.Images != null && dto.Images.Any())
            {
                _logger.LogInformation("Uploading {Count} new images for destination {DestinationId}", dto.Images.Count, id);

                var uploadResults = await _cloudinaryService.UploadMultipleImagesAsync(
                    dto.Images,
                    folder: "destinations",
                    maxConcurrency: 10
                );

                // Process successful uploads
                var uploadedImages = uploadResults
                    .Where(r => r.IsSuccess && r.Result != null)
                    .Select(r => r.Result.SecureUrl)
                    .ToList();

                // Append new images to existing ones
                foreach (var image in uploadedImages)
                {
                    destination.Images.Add(image);
                }

                _logger.LogInformation("Successfully uploaded and added {SuccessCount}/{TotalCount} images",
                    uploadedImages.Count, dto.Images.Count);
            }

            // Update FAQs if provided
            if (dto.FAQs != null)
            {
                // Remove existing FAQs
                _context.FAQs.RemoveRange(destination.FAQs);

                // Add new FAQs
                var newFaqs = _mapper.Map<List<FAQ>>(dto.FAQs);
                // Ensure IDs are 0 to be treated as new
                foreach (var faq in newFaqs)
                {
                    faq.Id = 0;
                    destination.FAQs.Add(faq);
                }
            }

            // Save changes
            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Destination updated successfully: {DestinationId}", id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Destinations.AnyAsync(e => e.Id == id))
                {
                    throw new KeyNotFoundException($"Destination with id {id} not found");
                }
                else
                {
                    throw;
                }
            }

            await InvalidateDestinationsCache();

            return _mapper.Map<DestinationDetailsResponse>(destination);
        }

        public async Task DeleteDestinationAsync(int id)
        {
            _logger.LogInformation("Deleting destination with id: {DestinationId}", id);

            var destination = await _context.Destinations.FindAsync(id);
            if (destination == null)
            {
                _logger.LogWarning("Destination not found: {DestinationId}", id);
                throw new KeyNotFoundException($"Destination with id {id} not found");
            }

            _context.Destinations.Remove(destination);
            await _context.SaveChangesAsync();

            await InvalidateDestinationsCache();

            _logger.LogInformation("Destination deleted successfully: {DestinationId}", id);
        }

        private string ExtractPublicIdFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;
            try
            {
                var uri = new Uri(url);
                var segments = uri.Segments;
                var uploadIndex = Array.IndexOf(segments, "upload/");
                if (uploadIndex >= 0 && segments.Length > uploadIndex + 2)
                {
                    var pathAfterVersion = string.Join("", segments.Skip(uploadIndex + 2));
                    var publicIdWithExt = Uri.UnescapeDataString(pathAfterVersion);
                    var extIndex = publicIdWithExt.LastIndexOf('.');
                    if (extIndex > 0)
                    {
                        return publicIdWithExt.Substring(0, extIndex);
                    }
                    return publicIdWithExt;
                }
            }
            catch { }
            return string.Empty;
        }
    }
}
