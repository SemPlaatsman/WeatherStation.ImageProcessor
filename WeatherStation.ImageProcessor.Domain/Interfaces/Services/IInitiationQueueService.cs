namespace WeatherStation.ImageProcessor.Domain.Interfaces.Services
{
    public interface IInitiationQueueService
    {
        Task EnqueueImageGenerationInitiationAsync(string jobId, CancellationToken cancellationToken);
    }
}
