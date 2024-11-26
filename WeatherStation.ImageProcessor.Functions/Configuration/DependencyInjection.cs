using Microsoft.Extensions.DependencyInjection;
using WeatherStation.ImageProcessor.Domain.Interfaces.Facades;
using WeatherStation.ImageProcessor.Domain.Interfaces.Repositories;
using WeatherStation.ImageProcessor.Domain.Interfaces.Services;
using WeatherStation.ImageProcessor.Infrastructure.Facades;
using WeatherStation.ImageProcessor.Infrastructure.Repositories;
using WeatherStation.ImageProcessor.Infrastructure.Services;

namespace WeatherStation.ImageProcessor.Functions.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services)
        {
            // Register facades
            services.AddScoped<IJobInitiationFacade, JobInitiationFacade>();

            // Register services
            services.AddScoped<IInitiationQueueService, InitiationQueueService>();

            // Register repositories
            services.AddScoped<IJobRepository, JobRepository>();

            return services;
        }
    }
}
