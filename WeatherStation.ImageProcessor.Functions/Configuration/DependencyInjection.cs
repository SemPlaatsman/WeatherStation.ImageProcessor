using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using WeatherStation.ImageProcessor.Domain.Interfaces.Clients;
using WeatherStation.ImageProcessor.Domain.Interfaces.Facades;
using WeatherStation.ImageProcessor.Domain.Interfaces.Repositories;
using WeatherStation.ImageProcessor.Domain.Interfaces.Services;
using WeatherStation.ImageProcessor.Infrastructure.Clients;
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
            // HTTP Clients
            services.AddHttpClient<IWeatherClient, BuienradarClient>(client =>
            {
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            });

            services.AddHttpClient<IImageClient, UnsplashClient>(client =>
            {
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            });

            // Register facades
            services.AddScoped<IJobInitiationFacade, JobInitiationFacade>();
            services.AddScoped<IWeatherJobFacade, WeatherJobFacade>();

            // Register services
            services.AddScoped<IInitiationQueueService, InitiationQueueService>();
            services.AddScoped<IProcessingQueueService, ProcessingQueueService>();
            services.AddScoped<IWeatherService, WeatherService>();

            // Register repositories
            services.AddScoped<IJobRepository, JobRepository>();

            return services;
        }
    }
}
