using AspNetCoreRateLimit;

namespace FanQuest.API.Configuration
{
    public static class RateLimitServiceExtensions
    {
        public static IServiceCollection AddRateLimiting(this IServiceCollection services, IConfiguration configuration)
        {
            // Memory cache is required for rate limiting
            services.AddMemoryCache();

            // Configure IP rate limiting
            services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
            services.Configure<IpRateLimitPolicies>(configuration.GetSection("IpRateLimitPolicies"));

            // Add rate limiting services (this includes IRateLimitConfiguration automatically)
            services.AddInMemoryRateLimiting();

            // Register processing strategy
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

            return services;
        }
    }
}
