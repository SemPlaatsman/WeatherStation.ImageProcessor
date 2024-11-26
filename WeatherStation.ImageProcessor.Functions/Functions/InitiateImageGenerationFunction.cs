using System;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using WeatherStation.ImageProcessor.Infrastructure.Models;

namespace WeatherStation.ImageProcessor.Functions.Functions
{
    public class InitiateImageGenerationFunction
    {
        private readonly ILogger<InitiateImageGenerationFunction> _logger;

        public InitiateImageGenerationFunction(ILogger<InitiateImageGenerationFunction> logger)
        {
            _logger = logger;
        }

        [Function(nameof(InitiateImageGenerationFunction))]
        public async Task RunAsync(
            [QueueTrigger("%Storage:InitiationQueueName%")] InitiateImageGenerationMessage message,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing initiation message for job: {JobId}", message.JobId);
            await Task.CompletedTask;
        }
    }
}
