using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using WeatherStation.ImageProcessor.Infrastructure.Models;

namespace WeatherStation.ImageProcessor.Functions.Functions
{
    public class ProcessWeatherImageFunction
    {
        private readonly ILogger<ProcessWeatherImageFunction> _logger;

        public ProcessWeatherImageFunction(ILogger<ProcessWeatherImageFunction> logger)
        {
            _logger = logger;
        }

        [Function(nameof(ProcessWeatherImageFunction))]
        public Task RunAsync(
            [QueueTrigger("%Storage:ProcessingQueueName%")] ProcessWeatherImageMessage message,
            FunctionContext context)
        {
            _logger.LogInformation(
                "Processing weather image for job {JobId}, station {StationId}",
                message.JobId,
                message.StationId);

            return Task.CompletedTask;
        }
    }
}