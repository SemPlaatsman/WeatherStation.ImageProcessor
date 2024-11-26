using Azure.Storage.Queues;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WeatherStation.ImageProcessor.Infrastructure.Options;
using WeatherStation.ImageProcessor.Infrastructure.Util;
using WeatherStation.ImageProcessor.Domain.Interfaces.Services;

namespace WeatherStation.ImageProcessor.Infrastructure.Services
{
    public class InitiationQueueService(
       IOptions<StorageOptions> options,
       ILogger<InitiationQueueService> logger) : IInitiationQueueService
    {
        private readonly QueueClient _queueClient = InitializeQueueClient(options.Value);

        public Task SendInitiateImageGenerationMessageAsync(string initiateImageGenerationMessage, CancellationToken cancellationToken) =>
            logger.ExecuteWithExceptionLoggingAsync(
                () => _queueClient.SendMessageAsync(initiateImageGenerationMessage, cancellationToken),
                "Error sending image generation initation message {MessageContent}",
                initiateImageGenerationMessage);

        private static QueueClient InitializeQueueClient(StorageOptions options)
        {
            var client = new QueueClient(
                options.QueueStorageConnection,
                options.InitiationQueueName,
                new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 });

            client.CreateIfNotExists();
            return client;
        }
    }
}
