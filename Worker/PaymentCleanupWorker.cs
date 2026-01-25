using Microsoft.EntityFrameworkCore;
using TravelTechApi.Data;

namespace TravelTechApi.Worker;

public class PaymentCleanupWorker : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IConfiguration _config;
    private readonly ILogger<PaymentCleanupWorker> _logger;

    public PaymentCleanupWorker(IServiceScopeFactory serviceScopeFactory, IConfiguration config, ILogger<PaymentCleanupWorker> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _config = config;
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var interval = TimeSpan.FromMinutes(_config.GetValue<int>("Cleanup:IntervalMinutes", 15));
        var batchSize = _config.GetValue<int>("Cleanup:BatchSize", 500);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var now = DateTime.UtcNow;

                var ids = await db.PaymentTransactions
                    .Where(p => p.Status == Entities.PaymentStatus.Pending && p.ExpiresAt <= now)
                    .OrderBy(p => p.ExpiresAt)
                    .Take(batchSize)
                    .Select(p => p.Id)
                    .ToListAsync(stoppingToken);

                if (ids.Count > 0)
                {
                    var updated = await db.PaymentTransactions
                        .Where(p => ids.Contains(p.Id))
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(p => p.Status,
                            Entities.PaymentStatus.Expired)
                            .SetProperty(p => p.UpdatedAt, now)
                        );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up payment transactions");
            }

            await Task.Delay(interval, stoppingToken);
        }
    }
}