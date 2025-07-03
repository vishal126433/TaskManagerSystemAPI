using Microsoft.Extensions.DependencyInjection;

namespace TaskManager.Extensions
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddAngularCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AngularApp", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
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
