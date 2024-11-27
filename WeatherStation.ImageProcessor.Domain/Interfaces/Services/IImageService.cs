using WeatherStation.ImageProcessor.Domain.Entities;

namespace WeatherStation.ImageProcessor.Domain.Interfaces.Services
{
    public interface IImageService
    {
        Task<WeatherImage> GetRandomImageAsync(CancellationToken cancellationToken = default);

        Task<Stream> DownloadImageAsync(
            string url,
            CancellationToken cancellationToken = default);
    }
}
