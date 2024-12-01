using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WeatherStation.ImageProcessor.Domain.Entities;
using WeatherStation.ImageProcessor.Domain.Enums;
using WeatherStation.ImageProcessor.Domain.Interfaces.Repositories;
using WeatherStation.ImageProcessor.Infrastructure.Options;
using WeatherStation.ImageProcessor.Infrastructure.Util;

namespace WeatherStation.ImageProcessor.Infrastructure.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly TableClient _tableClient;
        private readonly StorageOptions _options;
        private readonly ILogger<JobRepository> _logger;
        private static readonly Random Jitter = new();
        private const int MaxAttempts = 3;

        public JobRepository(IOptions<StorageOptions> options, ILogger<JobRepository> logger)
        {
            _options = options.Value;
            _logger = logger;
            TableServiceClient tableServiceClient = new(_options.TableStorageConnection);
            _tableClient = tableServiceClient.GetTableClient(_options.JobTableName);

            // Create the table if it doesn't exist
            _tableClient.CreateIfNotExists();
        }

        public async Task<WeatherJob> CreateJobAsync(WeatherJob job, CancellationToken cancellationToken = default)
        {
            TableEntity entity = new()
            {
                PartitionKey = _options.JobTableName,
                RowKey = job.Id,
                ["Status"] = job.Status,
                ["CreatedAt"] = job.CreatedAt,
                ["CompletedImages"] = job.CompletedImages,
                ["TotalImages"] = job.TotalImages,
                ["RequestedStations"] = job.RequestedStations
            };

            await _tableClient.AddEntityAsync(entity, cancellationToken);
            return job;
        }

        public async Task<WeatherJob?> GetJobAsync(string jobId, CancellationToken cancellationToken = default)
        {
            try
            {
                Response<TableEntity> response = await _tableClient.GetEntityAsync<TableEntity>(
                    _options.JobTableName, jobId, cancellationToken: cancellationToken);

                return new WeatherJob
                {
                    Id = response.Value.RowKey,
                    Status = response.Value.GetString("Status"),
                    CreatedAt = response.Value.GetDateTime("CreatedAt") ?? DateTime.Now,
                    CompletedImages = response.Value.GetInt32("CompletedImages") ?? 0,
                    TotalImages = response.Value.GetInt32("TotalImages"),
                    RequestedStations = response.Value.GetInt32("RequestedStations")
                };
            }
            catch (RequestFailedException)
            {
                return null;
            }
        }

        public async Task UpdateJobAsync(WeatherJob job, CancellationToken cancellationToken = default)
        {
            TableEntity entity = new()
            {
                PartitionKey = _options.JobTableName,
                RowKey = job.Id,
                ["Status"] = job.Status,
                ["CreatedAt"] = job.CreatedAt,
                ["CompletedImages"] = job.CompletedImages,
                ["TotalImages"] = job.TotalImages,
                ["RequestedStations"] = job.RequestedStations
            };

            await _tableClient.UpdateEntityAsync(entity, ETag.All, TableUpdateMode.Replace, cancellationToken);
        }

        /// <summary>
        /// Increments the CompletedImages counter for a job in a thread-safe manner.
        /// Uses optimistic concurrency control with retry logic to handle concurrent updates:
        /// 1. Reads current job state
        /// 2. Increments CompletedImages
        /// 3. Updates job status to Completed if all images are processed
        /// 4. Attempts to save with ETag check
        /// 5. If save fails due to concurrent update, retries with exponential backoff
        /// This ensures accurate counting even when multiple queue messages are processed simultaneously.
        /// </summary>
        public async Task IncrementCompletedImagesAsync(
            string jobId,
            CancellationToken cancellationToken = default)
        {
            byte attempt = 0;
            while (attempt < MaxAttempts)
            {
                try
                {
                    TableEntity entity = await GetAndValidateJobEntityAsync(jobId, cancellationToken);
                    (int completedImages, int totalImages) = ExtractImageCounts(entity);

                    UpdateJobStatus(entity, completedImages + 1, totalImages);
                    await _tableClient.UpdateEntityAsync(
                        entity,
                        entity.ETag,
                        TableUpdateMode.Replace,
                        cancellationToken);
                    return;
                }
                catch (RequestFailedException ex) when (ex.Status == 412)
                {
                    if (++attempt == MaxAttempts)
                    {
                        _logger.LogError("Failed to increment completed images after {Attempts} attempts for job {JobId}", MaxAttempts, jobId);
                        throw;
                    }
                    await Task.Delay(CalculateBackoffDelay(attempt), cancellationToken);
                }
            }
        }

        private Task<TableEntity> GetAndValidateJobEntityAsync(string jobId, CancellationToken cancellationToken) =>
            _logger.ExecuteWithExceptionLoggingAsync(
                async () =>
                {
                    Response<TableEntity> response = await _tableClient.GetEntityAsync<TableEntity>(_options.JobTableName, jobId, cancellationToken: cancellationToken);
                    if (response?.Value == null)
                        throw new InvalidOperationException($"Job {jobId} not found");
                    return response.Value;
                },
                $"Failed to get job {jobId}");

        private (int completed, int total) ExtractImageCounts(TableEntity entity) =>
            (ExtractIntValue(entity, "CompletedImages"), ExtractIntValue(entity, "TotalImages"));

        private int ExtractIntValue(TableEntity entity, string propertyName) =>
            _logger.ExecuteWithExceptionLoggingAsync(
                () =>
                {
                    if (!entity.TryGetValue(propertyName, out object? value) ||
                        value == null ||
                        !int.TryParse(value.ToString(), out int result))
                    {
                        throw new InvalidOperationException($"Invalid {propertyName} value for job {entity.RowKey}");
                    }
                    return Task.FromResult(result);
                },
                $"Failed to extract {propertyName} for job {entity.RowKey}").Result;

        private static void UpdateJobStatus(TableEntity entity, int newCompletedCount, int totalImages)
        {
            entity["CompletedImages"] = newCompletedCount;
            if (newCompletedCount == totalImages)
            {
                entity["Status"] = JobStatus.Completed.ToString();
            }
        }

        private static TimeSpan CalculateBackoffDelay(int attempt)
        {
            // Base delay with exponential backoff (50ms, 100ms, 200ms)
            double baseDelay = Math.Pow(2, attempt - 1) * 50;

            // Add random jitter between 0-50ms to prevent thundering herd
            // See: https://en.wikipedia.org/wiki/Thundering_herd_problem
            int jitter = Jitter.Next(0, 50);

            return TimeSpan.FromMilliseconds(baseDelay + jitter);
        }
    }
}
