using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;
using WeatherStation.ImageProcessor.Domain.Interfaces.Clients;
using WeatherStation.ImageProcessor.Infrastructure.External.Models.Buienradar;
using WeatherStation.ImageProcessor.Infrastructure.Options;

namespace WeatherStation.ImageProcessor.Infrastructure.Clients
{
    public class BuienradarClient : IWeatherClient
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<BuienradarOptions> _options;
        private readonly ILogger<BuienradarClient> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public BuienradarClient(
            HttpClient httpClient,
            IOptions<BuienradarOptions> options,
            ILogger<BuienradarClient> logger)
        {
            _httpClient = httpClient;
            _options = options;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<IEnumerable<Domain.Entities.WeatherStation>> GetWeatherDataAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Fetching weather data from Buienradar");

                BuienradarResponse? response = await _httpClient.GetFromJsonAsync<BuienradarResponse>(
                    _options.Value.Endpoint,
                    _jsonOptions,
                    cancellationToken);

                if (response?.Actual?.StationMeasurements == null)
                {
                    throw new Exception("Invalid response from Buienradar API");
                }

                List<Domain.Entities.WeatherStation> validStations = response
                    .Actual.StationMeasurements
                    .Where(IsValidStation)
                    .Select(MapToWeatherStation)
                    .ToList();

                _logger.LogInformation("Successfully fetched {Count} valid stations", validStations.Count);

                return validStations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch weather data from Buienradar");
                throw;
            }
        }

        public async Task<IEnumerable<Domain.Entities.WeatherStation>> GetWeatherDataAsync(
            IEnumerable<int> stationIds,
            CancellationToken cancellationToken = default)
        {
            IEnumerable<Domain.Entities.WeatherStation> allStations = await GetWeatherDataAsync(cancellationToken);
            return allStations.Where(s => stationIds.Contains(s.Id));
        }

        private static bool IsValidStation(StationMeasurement station) =>
            station.Temperature.HasValue &&
            station.FeelTemperature.HasValue &&
            station.WindSpeed.HasValue &&
            station.WindSpeedBft.HasValue &&
            station.Humidity.HasValue;

        private static Domain.Entities.WeatherStation MapToWeatherStation(StationMeasurement measurement) =>
            new(
                Id: measurement.StationId,
                Name: measurement.StationName,
                Region: measurement.Region ?? "Onbekend",
                Latitude: measurement.Latitude,
                Longitude: measurement.Longitude,
                Timestamp: measurement.Timestamp,
                WeatherDescription: measurement.WeatherDescription,
                IconUrl: measurement.IconUrl,
                Temperature: measurement.Temperature!.Value,
                FeelTemperature: measurement.FeelTemperature!.Value,
                WindSpeed: measurement.WindSpeed!.Value,
                WindSpeedBft: measurement.WindSpeedBft!.Value,
                WindDirection: measurement.WindDirection ?? "Onbekend",
                Humidity: measurement.Humidity!.Value,
                Precipitation: measurement.Precipitation,
                RainFallLastHour: measurement.RainFallLastHour,
                Visibility: measurement.Visibility
            );
    }
}