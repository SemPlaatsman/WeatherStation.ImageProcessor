using Microsoft.Extensions.Logging;
using WeatherStation.ImageProcessor.Domain.DTOs.Common;
using WeatherStation.ImageProcessor.Domain.DTOs.Response;
using WeatherStation.ImageProcessor.Domain.Enums;
using WeatherStation.ImageProcessor.Domain.Interfaces.Facades;
using WeatherStation.ImageProcessor.Domain.Interfaces.Repositories;

namespace WeatherStation.ImageProcessor.Infrastructure.Facades
{
    public class JobStatusFacade : IJobStatusFacade
    {
        private readonly IJobRepository _jobRepository;
        private readonly IImageRepository _imageRepository;
        private readonly ILogger<JobStatusFacade> _logger;

        public JobStatusFacade(
            IJobRepository jobRepository,
            IImageRepository imageRepository,
            ILogger<JobStatusFacade> logger)
        {
            _jobRepository = jobRepository;
            _imageRepository = imageRepository;
            _logger = logger;
        }

        public async Task<JobStatusResponse> GetJobStatusAsync(
            string jobId, CancellationToken cancellationToken = default)
        {
            var job = await _jobRepository.GetJobAsync(jobId, cancellationToken);

            if (job == null)
            {
                throw new KeyNotFoundException($"Job with ID {jobId} not found");
            }

            var response = new JobStatusResponse
            {
                Status = job.Status
            };

            if (job.Status != JobStatus.Initiated.ToString())
            {
                response.Progress = new ProgressInfo
                {
                    Completed = job.CompletedImages,
                    Total = job.TotalImages ?? 0
                };

                var images = await _imageRepository.GetImagesForJobAsync(jobId, cancellationToken);
                response.Images = images.Select(i => new ImageInfo
                {
                    StationId = i.StationId,
                    Url = i.Url
                }).ToList();
            }

            return response;
        }
    }
}
