using Microsoft.Extensions.Logging;
using WeatherStation.ImageProcessor.Domain.Entities;
using WeatherStation.ImageProcessor.Domain.Enums;
using WeatherStation.ImageProcessor.Domain.Interfaces.Facades;
using WeatherStation.ImageProcessor.Domain.Interfaces.Repositories;
using WeatherStation.ImageProcessor.Domain.Interfaces.Services;

namespace WeatherStation.ImageProcessor.Infrastructure.Facades
{
    public class JobInitiationFacade : IJobInitiationFacade
    {
        private readonly IJobRepository _jobRepository;
        private readonly IInitiationQueueService _initiationQueueService;
        private readonly ILogger<JobInitiationFacade> _logger;

        public JobInitiationFacade(
            IJobRepository jobRepository,
            IInitiationQueueService initiationQueueService,
            ILogger<JobInitiationFacade> logger)
        {
            _jobRepository = jobRepository;
            _initiationQueueService = initiationQueueService;
            _logger = logger;
        }

        public async Task<string> InitiateWeatherJobAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting weather job initiation process");

            var jobId = Guid.NewGuid().ToString();
            _logger.LogInformation("Generated new job ID: {JobId}", jobId);

            var job = new WeatherJob
            {
                Id = jobId,
                Status = JobStatus.Initiated.ToString(),
                CreatedAt = DateTime.UtcNow,
                CompletedImages = 0,
                TotalImages = null
            };

            await _jobRepository.CreateJobAsync(job, cancellationToken);
            _logger.LogInformation("Created new job with ID: {JobId}", jobId);

            await _initiationQueueService.EnqueueImageGenerationInitiationAsync(
                jobId, cancellationToken);

            _logger.LogInformation("Sent initiation message for job ID: {JobId}", jobId);

            return jobId;
        }
    }
}
