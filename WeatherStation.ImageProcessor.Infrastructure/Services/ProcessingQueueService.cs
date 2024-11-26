using Azure.Storage.Queues;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using WeatherStation.ImageProcessor.Domain.Interfaces.Services;
using WeatherStation.ImageProcessor.Infrastructure.Models;
using WeatherStation.ImageProcessor.Infrastructure.Options;
using WeatherStation.ImageProcessor.Infrastructure.Util;

namespace WeatherStation.ImageProcessor.Infrastructure.Services
{
    public class ProcessingQueueService : IProcessingQueueService
    {
        private readonly QueueClient _queueClient;
        private readonly ILogger<ProcessingQueueService> _logger;

        public ProcessingQueueService(
            IOptions<StorageOptions> options,
            ILogger<ProcessingQueueService> logger)
        {
            _logger = logger;
            _queueClient = InitializeQueueClient(options.Value);
        }

        public async Task EnqueueWeatherImageProcessingAsync(
            string jobId,
            int stationId,
            Domain.Entities.WeatherStation weatherData,
            CancellationToken cancellationToken)
        {
            var message = new ProcessWeatherImageMessage
            {
                JobId = jobId,
                StationId = stationId,
                WeatherData = weatherData
            };

            var messageJson = JsonSerializer.Serialize(message);

            await _logger.ExecuteWithExceptionLoggingAsync(
                () => _queueClient.SendMessageAsync(messageJson, cancellationToken),
                "Error sending process weather image message for station {StationId}",
                stationId);
        }

        private static QueueClient InitializeQueueClient(StorageOptions options)
        {
            var client = new QueueClient(
                options.QueueStorageConnection,
                options.ProcessingQueueName,
                new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 });

            client.CreateIfNotExists();
            return client;
        }
    }
}