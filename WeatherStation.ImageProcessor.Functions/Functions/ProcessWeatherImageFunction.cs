using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using WeatherStation.ImageProcessor.Domain.Interfaces.Facades;
using WeatherStation.ImageProcessor.Infrastructure.Models;

namespace WeatherStation.ImageProcessor.Functions.Functions
{
    public class ProcessWeatherImageFunction
    {
        private readonly IImageProcessingFacade _imageProcessingFacade;
        private readonly ILogger<ProcessWeatherImageFunction> _logger;

        public ProcessWeatherImageFunction(
            IImageProcessingFacade imageProcessingFacade,
            ILogger<ProcessWeatherImageFunction> logger)
        {
            _imageProcessingFacade = imageProcessingFacade;
            _logger = logger;
        }

        [Function(nameof(ProcessWeatherImageFunction))]
        public async Task RunAsync(
            [QueueTrigger("%Storage:ProcessingQueueName%")] ProcessWeatherImageMessage message,
            FunctionContext context)
        {
            _logger.LogInformation(
                "Processing weather image for job {JobId}, station {StationId}",
                message.JobId,
                message.WeatherData.Id);

            await _imageProcessingFacade.ProcessWeatherImageAsync(
                message.JobId,
                message.WeatherData,
                context.CancellationToken);
        }
    }
}