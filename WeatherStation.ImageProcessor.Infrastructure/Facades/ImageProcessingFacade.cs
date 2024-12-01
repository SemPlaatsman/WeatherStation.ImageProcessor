using Microsoft.Extensions.Logging;
using WeatherStation.ImageProcessor.Domain.Entities;
using WeatherStation.ImageProcessor.Domain.Enums;
using WeatherStation.ImageProcessor.Domain.Interfaces.Facades;
using WeatherStation.ImageProcessor.Domain.Interfaces.Repositories;
using WeatherStation.ImageProcessor.Domain.Interfaces.Services;
using WeatherStation.ImageProcessor.Infrastructure.Util;

namespace WeatherStation.ImageProcessor.Infrastructure.Facades
{
    public class ImageProcessingFacade : IImageProcessingFacade
    {
        private readonly IImageService _imageService;
        private readonly IImageGenerationService _imageGenerationService;
        private readonly IImageRepository _imageRepository;
        private readonly IJobRepository _jobRepository;
        private readonly ILogger<ImageProcessingFacade> _logger;

        public ImageProcessingFacade(
            IImageService imageService,
            IImageGenerationService imageGenerationService,
            IImageRepository imageRepository,
            IJobRepository jobRepository,
            ILogger<ImageProcessingFacade> logger)
        {
            _imageService = imageService;
            _imageGenerationService = imageGenerationService;
            _imageRepository = imageRepository;
            _jobRepository = jobRepository;
            _logger = logger;
        }

        public Task ProcessWeatherImageAsync(
            string jobId,
            Domain.Entities.WeatherStation weatherStation,
            CancellationToken cancellationToken = default) =>
            _logger.ExecuteWithExceptionLoggingAsync(
                async () =>
                {
                    // Immediately get job because if it doesn't exist, processing is not needed
                    WeatherJob? job = await _jobRepository.GetJobAsync(jobId, cancellationToken);

                    if (job == null)
                    {
                        _logger.LogWarning("Job {JobId} not found, skipping processing", jobId);
                        return;
                    }

                    // Get and download random image
                    WeatherImage weatherImage = await _imageService.GetRandomImageAsync(cancellationToken);
                    Stream baseImageStream = await _imageService.DownloadImageAsync(weatherImage.Url, cancellationToken);

                    using (baseImageStream)
                    {
                        // Generate weather image
                        using Stream weatherImageStream = await _imageGenerationService.GenerateWeatherImageAsync(
                            baseImageStream,
                            weatherStation,
                            weatherImage.Metadata.Attribution,
                            cancellationToken);

                        // Save to blob storage
                        await _imageRepository.SaveImageAsync(
                            jobId,
                            weatherStation.Id.ToString(),
                            weatherImageStream,
                            cancellationToken);
                    }

                    // Update completed images with retries and optimistic concurrency
                    await _jobRepository.IncrementCompletedImagesAsync(jobId, cancellationToken);
                },
                "Failed to process weather image for job {JobId}, station {StationId}",
                jobId,
                weatherStation.Id);
    }
}
