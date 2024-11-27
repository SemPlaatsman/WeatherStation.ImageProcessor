using Microsoft.Extensions.Logging;
using WeatherStation.ImageProcessor.Domain.Entities;
using WeatherStation.ImageProcessor.Domain.Interfaces.Clients;
using WeatherStation.ImageProcessor.Domain.Interfaces.Services;
using WeatherStation.ImageProcessor.Infrastructure.Util;

namespace WeatherStation.ImageProcessor.Infrastructure.Services
{
    public class ImageService : IImageService
    {
        private readonly IImageClient _imageClient;
        private readonly ILogger<ImageService> _logger;

        public ImageService(
            IImageClient imageClient,
            ILogger<ImageService> logger)
        {
            _imageClient = imageClient;
            _logger = logger;
        }

        public Task<WeatherImage> GetRandomImageAsync(CancellationToken cancellationToken = default)
            => _logger.ExecuteWithExceptionLoggingAsync(
                () => _imageClient.GetRandomImageAsync(cancellationToken),
                "Could not get random image");

        public Task<Stream> DownloadImageAsync(string url, CancellationToken cancellationToken = default)
            => _logger.ExecuteWithExceptionLoggingAsync(
                () => _imageClient.DownloadImageAsync(url, cancellationToken),
                "Could not download image for url: {URL}",
                url);
    }
}
