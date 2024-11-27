namespace WeatherStation.ImageProcessor.Domain.Interfaces.Facades
{
    public interface IJobInitiationFacade
    {
        Task<string> InitiateWeatherJobAsync(
            int? numberOfStations = null,
            CancellationToken cancellationToken = default);
    }
}
