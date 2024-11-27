namespace WeatherStation.ImageProcessor.Domain.Interfaces.Services
{
    public interface IWeatherService
    {
        Task<IEnumerable<Entities.WeatherStation>> GetWeatherStationsAsync(
            int? numberOfStations = null,
            CancellationToken cancellationToken = default);

        Task<Entities.WeatherStation?> GetWeatherStationAsync(
            int stationId,
            CancellationToken cancellationToken = default);
    }
}