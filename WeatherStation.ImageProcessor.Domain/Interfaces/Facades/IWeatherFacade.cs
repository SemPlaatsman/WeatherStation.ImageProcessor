namespace WeatherStation.ImageProcessor.Domain.Interfaces.Facades
{
    public interface IWeatherJobFacade
    {
        Task ProcessJobAsync(
            string jobId,
            CancellationToken cancellationToken = default);
    }
}