using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using WeatherStation.ImageProcessor.Domain.Interfaces.Facades;
using WeatherStation.ImageProcessor.Infrastructure.Models;

namespace WeatherStation.ImageProcessor.Functions.Functions
{
    public class InitiateImageGenerationFunction
    {
        private readonly IWeatherJobFacade _weatherJobFacade;
        private readonly ILogger<InitiateImageGenerationFunction> _logger;

        public InitiateImageGenerationFunction(
            IWeatherJobFacade weatherJobFacade,
            ILogger<InitiateImageGenerationFunction> logger)
        {
            _weatherJobFacade = weatherJobFacade;
            _logger = logger;
        }

        [Function(nameof(InitiateImageGenerationFunction))]
        public async Task RunAsync(
            [QueueTrigger("%Storage:InitiationQueueName%")] InitiateImageGenerationMessage message,
            FunctionContext context)
        {
            _logger.LogInformation("Processing initiation message for job {JobId}", message.JobId);
            await _weatherJobFacade.ProcessJobAsync(message.JobId);
            _logger.LogInformation("Completed processing initiation message for job {JobId}", message.JobId);
        }
    }
}