namespace WeatherStation.ImageProcessor.Domain.Interfaces.Facades
{
    public interface IImageProcessingFacade
    {
        Task ProcessWeatherImageAsync(
            string jobId,
            Entities.WeatherStation weatherStation,
            CancellationToken cancellationToken = default);
    }
}
