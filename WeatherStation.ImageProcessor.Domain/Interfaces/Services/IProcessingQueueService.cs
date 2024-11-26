namespace WeatherStation.ImageProcessor.Domain.Interfaces.Services
{
    public interface IProcessingQueueService
    {
        Task EnqueueWeatherImageProcessingAsync(
            string jobId,
            int stationId,
            Entities.WeatherStation weatherData,
            CancellationToken cancellationToken = default);
    }
}