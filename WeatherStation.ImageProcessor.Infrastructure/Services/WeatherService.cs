using Microsoft.Extensions.Logging;
using WeatherStation.ImageProcessor.Domain.Interfaces.Clients;
using WeatherStation.ImageProcessor.Domain.Interfaces.Services;
using WeatherStation.ImageProcessor.Infrastructure.Util;

namespace WeatherStation.ImageProcessor.Infrastructure.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IWeatherClient _weatherClient;
        private readonly ILogger<WeatherService> _logger;

        public WeatherService(
            IWeatherClient weatherClient,
            ILogger<WeatherService> logger)
        {
            _weatherClient = weatherClient;
            _logger = logger;
        }

        public Task<IEnumerable<Domain.Entities.WeatherStation>> GetAllWeatherStationsAsync(
            CancellationToken cancellationToken = default) =>
            _logger.ExecuteWithExceptionLoggingAsync(
                () => _weatherClient.GetWeatherDataAsync(cancellationToken),
                "Failed to get weather data for all stations");

        public Task<Domain.Entities.WeatherStation?> GetWeatherStationAsync(
            int stationId,
            CancellationToken cancellationToken = default) =>
            _logger.ExecuteWithExceptionLoggingAsync(
                async () => {
                    var stations = await _weatherClient.GetWeatherDataAsync(
                        new[] { stationId },
                        cancellationToken);
                    return stations.FirstOrDefault();
                },
                "Failed to get weather data for station {StationId}",
                stationId);
    }
}