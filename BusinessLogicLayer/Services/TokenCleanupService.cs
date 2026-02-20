using DataAccessLayer.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BusinessLogicLayer.Services
{
    public class TokenCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TokenCleanupService> _logger;
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromHours(24);
        public TokenCleanupService(IServiceProvider serviceProvider, ILogger<TokenCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Token Blacklist Cleanup task starting...");
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var expiredBlacklist = context.TokenBlacklists.Where(t => t.ExpirationDate < DateTime.UtcNow);
                    context.TokenBlacklists.RemoveRange(expiredBlacklist);

                    var oldRefreshTokens = context.RefreshTokens.Where(t=>t.DateExpires < DateTime.UtcNow.AddDays(-30));

                    context.RefreshTokens.RemoveRange(oldRefreshTokens);
                    await context.SaveChangesAsync(stoppingToken);
                }

                _logger.LogInformation("Token Blacklist cleanup task completed.");

                await Task.Delay(_cleanupInterval, stoppingToken);
            }
        }
    }
}
