using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using WeatherStation.ImageProcessor.Domain.Entities;
using WeatherStation.ImageProcessor.Domain.Interfaces.Repositories;
using WeatherStation.ImageProcessor.Infrastructure.Options;

namespace WeatherStation.ImageProcessor.Infrastructure.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly TableClient _tableClient;
        private readonly StorageOptions _options;

        public JobRepository(IOptions<StorageOptions> options)
        {
            _options = options.Value;
            var tableServiceClient = new TableServiceClient(_options.TableStorageConnection);
            _tableClient = tableServiceClient.GetTableClient(_options.JobTableName);

            // Create the table if it doesn't exist
            _tableClient.CreateIfNotExists();
        }

        public async Task<WeatherJob> CreateJobAsync(WeatherJob job, CancellationToken cancellationToken = default)
        {
            var entity = new TableEntity
            {
                PartitionKey = _options.JobTableName,
                RowKey = job.Id,
                ["Status"] = job.Status,
                ["CreatedAt"] = job.CreatedAt,
                ["CompletedImages"] = job.CompletedImages,
                ["TotalImages"] = job.TotalImages
            };

            await _tableClient.AddEntityAsync(entity, cancellationToken);
            return job;
        }

        public async Task<WeatherJob?> GetJobAsync(string jobId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _tableClient.GetEntityAsync<TableEntity>(
                    _options.JobTableName, jobId, cancellationToken: cancellationToken);

                return new WeatherJob
                {
                    Id = response.Value.RowKey,
                    Status = response.Value.GetString("Status"),
                    CreatedAt = response.Value.GetDateTime("CreatedAt") ?? DateTime.Now,
                    CompletedImages = response.Value.GetInt32("CompletedImages") ?? 0,
                    TotalImages = response.Value.GetInt32("TotalImages")
                };
            }
            catch (RequestFailedException)
            {
                return null;
            }
        }

        public async Task UpdateJobAsync(WeatherJob job, CancellationToken cancellationToken = default)
        {
            var entity = new TableEntity
            {
                PartitionKey = _options.JobTableName,
                RowKey = job.Id,
                ["Status"] = job.Status,
                ["CreatedAt"] = job.CreatedAt,
                ["CompletedImages"] = job.CompletedImages,
                ["TotalImages"] = job.TotalImages
            };

            await _tableClient.UpdateEntityAsync(entity, ETag.All, TableUpdateMode.Replace, cancellationToken);
        }
    }
}
