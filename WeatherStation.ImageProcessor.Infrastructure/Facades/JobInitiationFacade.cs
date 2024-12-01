using Microsoft.Extensions.Logging;
using WeatherStation.ImageProcessor.Domain.Entities;
using WeatherStation.ImageProcessor.Domain.Enums;
using WeatherStation.ImageProcessor.Domain.Interfaces.Facades;
using WeatherStation.ImageProcessor.Domain.Interfaces.Repositories;
using WeatherStation.ImageProcessor.Domain.Interfaces.Services;

namespace WeatherStation.ImageProcessor.Infrastructure.Facades
{
    public class JobInitiationFacade(
        IJobRepository jobRepository,
        IInitiationQueueService initiationQueueService,
        ILogger<JobInitiationFacade> logger) : IJobInitiationFacade
    {
        private readonly IJobRepository _jobRepository = jobRepository;
        private readonly IInitiationQueueService _initiationQueueService = initiationQueueService;
        private readonly ILogger<JobInitiationFacade> _logger = logger;

        public async Task<string> InitiateWeatherJobAsync(
            int? numberOfStations = null, 
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting weather job initiation process. RequestedStations: {RequestedStations}",
                numberOfStations?.ToString() ?? "all");

            string jobId = Guid.NewGuid().ToString();
            _logger.LogInformation("Generated new job ID: {JobId}", jobId);

            WeatherJob job = new()
            {
                Id = jobId,
                Status = JobStatus.Initiated.ToString(),
                CreatedAt = DateTime.UtcNow,
                CompletedImages = 0,
                TotalImages = null,
                RequestedStations = numberOfStations
            };

            await _jobRepository.CreateJobAsync(job, cancellationToken);
            _logger.LogInformation("Created new job with ID: {JobId}", jobId);

            await _initiationQueueService.EnqueueImageGenerationInitiationAsync(
                jobId, numberOfStations, cancellationToken);

            _logger.LogInformation("Sent initiation message for job ID: {JobId}", jobId);
            return jobId;
        }
    }
}
