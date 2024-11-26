namespace WeatherStation.ImageProcessor.Domain.Interfaces.Clients
{
    public interface IWeatherClient
    {
        Task<IEnumerable<Entities.WeatherStation>> GetWeatherDataAsync(
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Entities.WeatherStation>> GetWeatherDataAsync(
            IEnumerable<int> stationIds,
            CancellationToken cancellationToken = default);
    }
}
