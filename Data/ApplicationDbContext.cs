using AutoMapper.Execution;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TravelTechApi.Entities;

namespace TravelTechApi.Data
{
    /// <summary>
    /// Application database context with Identity
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Refresh tokens table
        /// </summary>
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Destination> Destinations { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<FAQ> FAQs { get; set; }
        public DbSet<DestinationSharing> DestinationSharings { get; set; }
        public DbSet<CloudinaryFileInfo> CloudinaryFileInfos { get; set; }

        // Travel Plan entities
        public DbSet<Plan> Plans { get; set; }
        public DbSet<DailyItinerary> DailyItineraries { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<FoodRecommendation> FoodRecommendations { get; set; }
        public DbSet<AccommodationRecommendation> AccommodationRecommendations { get; set; }
        public DbSet<TransportationRecommendation> TransportationRecommendations { get; set; }
        public DbSet<TravelHobby> TravelHobbies { get; set; }
        public DbSet<PriceSetting> PriceSettings { get; set; }

        // Subscription Plan entities
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<UserPlanSubscription> UserPlanSubscriptions { get; set; }

        // Contact entities
        public DbSet<ContactTopic> ContactTopics { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }

        // Payment
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<Giftcode> Giftcodes { get; set; }

        public DbSet<WebsiteFeedback> WebsiteFeedbacks { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Enable unaccent extension for case-insensitive and accent-insensitive search
            builder.HasPostgresExtension("unaccent");

            // Configure RefreshToken
            builder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Token).IsRequired().HasMaxLength(500);
                entity.Property(e => e.JwtId).IsRequired().HasMaxLength(500);
                entity.HasIndex(e => e.Token).IsUnique();

                // Relationship with ApplicationUser
                entity.HasOne(e => e.User)
                    .WithMany(u => u.RefreshTokens)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure ApplicationUser
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.FirstName).HasMaxLength(100);
                entity.Property(e => e.LastName).HasMaxLength(100);
                entity.Property(e => e.Gender).HasConversion<string>();

                entity.HasOne(e => e.Avatar)
                    .WithMany()
                    .HasForeignKey(e => e.AvatarId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure Destination
            builder.Entity<Destination>(entity =>
            {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).IsRequired();
                entity.Property(e => e.History).IsRequired();
                entity.Property(e => e.Lat).IsRequired();
                entity.Property(e => e.Lon).IsRequired();
                entity.Property(e => e.VideoUrl).IsRequired();
                entity.Property(e => e.Tags).IsRequired();
                entity.Property(e => e.LocationId).IsRequired();
                entity.HasOne(e => e.Location)
                    .WithMany(l => l.Destinations)
                    .HasForeignKey(e => e.LocationId)
                    .OnDelete(DeleteBehavior.Cascade);

                // CloudinaryFileInfo have no navigation property to Destination
                // So you have to cascade delete manually
                // entity.HasMany(e => e.Images)
                //     .WithOne(i => i.Destination)
                //     .HasForeignKey(e => e.DestinationId)
                //     .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Location
            builder.Entity<Location>(entity =>
            {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.RegionId).IsRequired();
                entity.HasOne(e => e.Region)
                    .WithMany(r => r.Locations)
                    .HasForeignKey(e => e.RegionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Region
            builder.Entity<Region>(entity =>
            {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            });

            // Configure FAQ
            builder.Entity<FAQ>(entity =>
            {
                entity.Property(e => e.Question).IsRequired();
                entity.Property(e => e.Answer).IsRequired();
                entity.Property(e => e.DestinationId).IsRequired();
                entity.HasOne(e => e.Destination)
                    .WithMany(d => d.FAQs)
                    .HasForeignKey(e => e.DestinationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure DestinationSharing
            builder.Entity<DestinationSharing>(entity =>
            {
                entity.Property(e => e.Comment).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired();
                entity.Property(e => e.DestinationId).IsRequired();
                entity.HasOne(e => e.Destination)
                    .WithMany(d => d.Sharings)
                    .HasForeignKey(e => e.DestinationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure CloudinaryFileInfo
            builder.Entity<CloudinaryFileInfo>(entity =>
            {
                entity.Property(e => e.PublicId).IsRequired();
                entity.Property(e => e.Url).IsRequired();
                entity.Property(e => e.ResourceType).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
            });

            // Configure Plan
            builder.Entity<Plan>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.IsSaved).HasDefaultValue(false);
                entity.Property(e => e.AIModel).HasMaxLength(100);

                // Destination location
                entity.HasOne(e => e.Location)
                    .WithMany()
                    .HasForeignKey(e => e.LocationId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Current location (optional)
                entity.HasOne(e => e.CurrentLocation)
                    .WithMany()
                    .HasForeignKey(e => e.CurrentLocationId)
                    .OnDelete(DeleteBehavior.Restrict);

                // User
                entity.HasOne(e => e.ApplicationUser)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // PriceSetting
                entity.HasOne(e => e.PriceSetting)
                    .WithMany()
                    .HasForeignKey(e => e.PriceSettingId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure DailyItinerary
            builder.Entity<DailyItinerary>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Summary).IsRequired();

                entity.HasOne(e => e.Plan)
                    .WithMany(p => p.DailyItineraries)
                    .HasForeignKey(e => e.PlanId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Activity
            builder.Entity<Activity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).IsRequired();

                entity.HasOne(e => e.DailyItinerary)
                    .WithMany(d => d.Activities)
                    .HasForeignKey(e => e.DailyItineraryId)
                    .OnDelete(DeleteBehavior.Cascade);

                // TODO: Uncomment when implementing Destination linking
                // entity.HasOne(e => e.Destination)
                //     .WithMany()
                //     .HasForeignKey(e => e.DestinationId)
                //     .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure FoodRecommendation
            builder.Entity<FoodRecommendation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.MealType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.DishName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.RestaurantName).IsRequired().HasMaxLength(200);

                entity.HasOne(e => e.DailyItinerary)
                    .WithMany(d => d.FoodRecommendations)
                    .HasForeignKey(e => e.DailyItineraryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure AccommodationRecommendation
            builder.Entity<AccommodationRecommendation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AccommodationType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);

                entity.HasOne(e => e.Plan)
                    .WithMany(p => p.AccommodationRecommendations)
                    .HasForeignKey(e => e.PlanId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure TransportationRecommendation
            builder.Entity<TransportationRecommendation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TransportationType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Route).IsRequired().HasMaxLength(500);

                entity.HasOne(e => e.Plan)
                    .WithMany(p => p.TransportationRecommendations)
                    .HasForeignKey(e => e.PlanId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure TravelHobby
            builder.Entity<TravelHobby>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).IsRequired();
            });

            // Configure SubscriptionPlan
            builder.Entity<SubscriptionPlan>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).IsRequired();
                entity.Property(e => e.Price).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.Order).IsRequired();
                entity.Property(e => e.DailyLimit).IsRequired();
            });

            // Configure UserPlanSubscription (Join table for many-to-many)
            builder.Entity<UserPlanSubscription>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.StartDate).IsRequired();
                entity.Property(e => e.EndDate).IsRequired();

                // Relationship with ApplicationUser
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Subscriptions)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relationship with SubscriptionPlan
                entity.HasOne(e => e.SubscriptionPlan)
                    .WithMany(p => p.Users)
                    .HasForeignKey(e => e.SubscriptionPlanId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure ContactTopic
            builder.Entity<ContactTopic>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);

                entity.HasMany(e => e.ContactMessages)
                    .WithOne(m => m.ContactTopic)
                    .HasForeignKey(m => m.ContactTopicId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure ContactMessage
            builder.Entity<ContactMessage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(254);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.Message).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.ContactTopic)
                    .WithMany(t => t.ContactMessages)
                    .HasForeignKey(e => e.ContactTopicId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure PaymentTransaction
            builder.Entity<PaymentTransaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OrderCode).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.OrderCode).IsUnique();
                entity.Property(e => e.Amount).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Status).HasConversion<string>();
                entity.Property(e => e.Gateway);
                entity.Property(e => e.TransactionId);
                entity.Property(e => e.TransactionDate);
                entity.Property(e => e.Content).HasMaxLength(500);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.SubscriptionPlan)
                    .WithMany()
                    .HasForeignKey(e => e.SubscriptionPlanId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Giftcode
            builder.Entity<Giftcode>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Code).IsUnique();
                entity.Property(e => e.MaximumDiscountAmount).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.DiscountPercentage).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500);
            });

            // Configure WebsiteFeedback
            builder.Entity<WebsiteFeedback>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Rating).IsRequired();
                entity.Property(e => e.AiTripPlannerUsefulness).IsRequired();
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.WouldRecommend).IsRequired();
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.SetNull);
                entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configure AuditLog
            builder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
                entity.Property(e => e.EntityName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.IpAddress).HasMaxLength(50);
                entity.HasIndex(e => e.CreatedAt); // Index for performance when querying logs
                entity.HasIndex(e => e.UserId);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure Blog
            builder.Entity<Blog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.Tags).IsRequired();

                entity.HasOne(e => e.Author)
                    .WithMany(u => u.Blogs)
                    .HasForeignKey(e => e.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
