namespace FanQuest.API.Configuration
{
    using AspNetCoreRateLimit;

    namespace FanQuest.API.Configuration
    {
        public static class RateLimitConfiguration
        {
            public static IServiceCollection AddRateLimiting(this IServiceCollection services, IConfiguration configuration)
            {
                // Load configuration from appsettings.json
                services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
                services.Configure<IpRateLimitPolicies>(configuration.GetSection("IpRateLimitPolicies"));

                // Inject counter and rules stores
                services.AddMemoryCache();
                services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
                services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
                services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
                services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

                return services;
            }
        }
    }
}
