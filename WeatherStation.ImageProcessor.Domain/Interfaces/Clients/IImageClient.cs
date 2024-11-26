using WeatherStation.ImageProcessor.Domain.Entities;

namespace WeatherStation.ImageProcessor.Domain.Interfaces.Clients
{
    public interface IImageClient
    {
        Task<WeatherImage> GetRandomImageAsync(
            string orientation,
            CancellationToken cancellationToken = default);

        Task<Stream> DownloadImageAsync(
            string url,
            CancellationToken cancellationToken = default);
    }
}
