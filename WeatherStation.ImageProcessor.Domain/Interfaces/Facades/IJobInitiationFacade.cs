namespace WeatherStation.ImageProcessor.Domain.Interfaces.Facades
{
    public interface IJobInitiationFacade
    {
        Task<string> InitiateWeatherJobAsync(CancellationToken cancellationToken = default);
    }
}
