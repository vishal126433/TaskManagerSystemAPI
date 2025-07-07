using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TaskManager.Extensions
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddAngularCors(this IServiceCollection services, IConfiguration configuration)
        {
            var corsSection = configuration.GetSection("Cors");
            var policyName = corsSection.GetValue<string>("PolicyName");
            var allowedOrigins = corsSection.GetSection("AllowedOrigins").Get<string[]>();

            services.AddCors(options =>
            {
                options.AddPolicy(policyName, policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials()
                          .WithExposedHeaders("Set-Cookie", "Authorization", "Expires");
                });
            });

            return services;
        }
    }
}
