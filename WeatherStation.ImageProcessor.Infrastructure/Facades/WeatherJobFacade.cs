using Microsoft.Extensions.Logging;
using WeatherStation.ImageProcessor.Domain.Entities;
using WeatherStation.ImageProcessor.Domain.Enums;
using WeatherStation.ImageProcessor.Domain.Interfaces.Facades;
using WeatherStation.ImageProcessor.Domain.Interfaces.Repositories;
using WeatherStation.ImageProcessor.Domain.Interfaces.Services;
using WeatherStation.ImageProcessor.Infrastructure.Util;

namespace WeatherStation.ImageProcessor.Infrastructure.Facades
{
    public class WeatherJobFacade : IWeatherJobFacade
    {
        private readonly IWeatherService _weatherService;
        private readonly IJobRepository _jobRepository;
        private readonly IProcessingQueueService _processingQueueService;
        private readonly ILogger<WeatherJobFacade> _logger;

        public WeatherJobFacade(
            IWeatherService weatherService,
            IJobRepository jobRepository,
            IProcessingQueueService processingQueueService,
            ILogger<WeatherJobFacade> logger)
        {
            _weatherService = weatherService;
            _jobRepository = jobRepository;
            _processingQueueService = processingQueueService;
            _logger = logger;
        }

        public Task ProcessJobAsync(string jobId, CancellationToken cancellationToken = default) =>
            _logger.ExecuteWithExceptionLoggingAsync(async () =>
            {
                // Get job
                WeatherJob? job = await _jobRepository.GetJobAsync(jobId, cancellationToken);
                if (job == null)
                {
                    throw new InvalidOperationException($"Job {jobId} not found");
                }

                // Get weather stations based on requested count
                List<Domain.Entities.WeatherStation> stations = (await _weatherService.GetWeatherStationsAsync(
                    job.RequestedStations,
                    cancellationToken)).ToList();

                // Update job
                job.Status = JobStatus.Processing.ToString();
                job.TotalImages = stations.Count;
                await _jobRepository.UpdateJobAsync(job, cancellationToken);

                // Queue processing messages
                foreach (Domain.Entities.WeatherStation station in stations)
                {
                    await _processingQueueService.EnqueueWeatherImageProcessingAsync(
                        jobId,
                        station.Id,
                        station,
                        cancellationToken);
                }
            },
            "Failed to process weather job {JobId}",
            jobId);
    }
}