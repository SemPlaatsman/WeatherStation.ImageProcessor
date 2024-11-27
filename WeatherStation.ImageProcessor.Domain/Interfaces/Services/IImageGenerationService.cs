namespace WeatherStation.ImageProcessor.Domain.Interfaces.Services
{
    public interface IImageGenerationService
    {
        Task<Stream> GenerateWeatherImageAsync(
            Stream baseImageStream,
            Entities.WeatherStation weatherStation,
            string attribution,
            CancellationToken cancellationToken = default);
    }
}
