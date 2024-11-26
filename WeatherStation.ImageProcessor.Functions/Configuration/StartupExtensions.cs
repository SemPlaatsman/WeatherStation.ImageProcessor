using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeatherStation.ImageProcessor.Infrastructure.Options;

namespace WeatherStation.ImageProcessor.Functions.Configuration
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddApplicationConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<ApiOptions>(
                configuration.GetSection(ApiOptions.SectionName));

            services.Configure<StorageOptions>(
                configuration.GetSection(StorageOptions.SectionName));

            return services;
        }
    }
}
