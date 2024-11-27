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

        public async Task<IEnumerable<Domain.Entities.WeatherStation>> GetWeatherStationsAsync(
            int? numberOfStations = null,
            CancellationToken cancellationToken = default) =>
            await _logger.ExecuteWithExceptionLoggingAsync(
                async () =>
                {
                    var allStations = await _weatherClient.GetWeatherDataAsync(cancellationToken);
                    if (!numberOfStations.HasValue)
                        return allStations;
                    var stationsList = allStations.ToList();
                    var requestedCount = Math.Min(numberOfStations.Value, stationsList.Count);
                    return stationsList
                        .OrderBy(_ => Random.Shared.Next())
                        .Take(requestedCount);
                },
                "Failed to get weather data. RequestedStations: {RequestedStations}",
                numberOfStations?.ToString() ?? "all");

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