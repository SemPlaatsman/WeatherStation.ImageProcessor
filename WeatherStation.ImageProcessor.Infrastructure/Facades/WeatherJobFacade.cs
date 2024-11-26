using Microsoft.Extensions.Logging;
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
                // Get weather data
                var stations = await _weatherService.GetAllWeatherStationsAsync(cancellationToken);
                var stationsList = stations.ToList();

                // Update job status
                var job = await _jobRepository.GetJobAsync(jobId, cancellationToken);
                if (job == null)
                {
                    throw new InvalidOperationException($"Job {jobId} not found");
                }

                job.Status = JobStatus.Processing.ToString();
                job.TotalImages = stationsList.Count;
                await _jobRepository.UpdateJobAsync(job, cancellationToken);

                // Queue processing messages
                foreach (var station in stationsList)
                {
                    await _processingQueueService.EnqueueWeatherImageProcessingAsync(
                        jobId,
                        station.StationId,
                        station,
                        cancellationToken);
                }
            },
            "Failed to process weather job {JobId}",
            jobId);
    }
}