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

            services.Configure<BuienradarOptions>(
                configuration.GetSection(BuienradarOptions.SectionName));

            services.Configure<UnsplashOptions>(
                configuration.GetSection(UnsplashOptions.SectionName));

            return services;
        }
    }
}
