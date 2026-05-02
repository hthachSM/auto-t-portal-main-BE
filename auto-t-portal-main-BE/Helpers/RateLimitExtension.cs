using AspNetCoreRateLimit;

namespace auto_t_portal_main_BE.Helpers;

public static class RateLimitExtension
{
    public static IServiceCollection AddRateLimiting(this IServiceCollection services, IConfiguration config)
    {
        services.AddMemoryCache();
        services.Configure<IpRateLimitOptions>(options =>
        {
            options.EnableEndpointRateLimiting = true;
            options.StackBlockedRequests = false;
            options.HttpStatusCode = 429;
            options.RealIpHeader = "X-Real-IP";
            options.GeneralRules =
            [
                new RateLimitRule
                {
                    Endpoint = "POST:/api/auth/login",
                    Period = "5m",
                    Limit = 10
                },
                new RateLimitRule
                {
                    Endpoint = "POST:/api/auth/register",
                    Period = "1h",
                    Limit = 5
                },
                new RateLimitRule
                {
                    Endpoint = "POST:/api/ai/*",
                    Period = "1m",
                    Limit = 20
                },
                new RateLimitRule
                {
                    Endpoint = "*",
                    Period = "1s",
                    Limit = 30
                }
            ];
        });

        services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
        services.AddInMemoryRateLimiting();

        return services;
    }
}
