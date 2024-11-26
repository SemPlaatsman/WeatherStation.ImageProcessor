namespace WeatherStation.ImageProcessor.Domain.Interfaces.Services
{
    public interface IInitiationQueueService
    {
        Task SendInitiateImageGenerationMessageAsync(string initiateImageGenerationMessage, CancellationToken cancellationToken);
    }
}
