namespace WeatherStation.ImageProcessor.Domain.Interfaces.Services
{
    public interface IWeatherService
    {
        Task<IEnumerable<Entities.WeatherStation>> GetAllWeatherStationsAsync(
            CancellationToken cancellationToken = default);

        Task<Entities.WeatherStation?> GetWeatherStationAsync(
            int stationId,
            CancellationToken cancellationToken = default);
    }
}