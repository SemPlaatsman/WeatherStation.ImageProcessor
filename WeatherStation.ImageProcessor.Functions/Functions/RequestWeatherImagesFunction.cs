using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using WeatherStation.ImageProcessor.Domain.Entities;
using WeatherStation.ImageProcessor.Domain.Enums;
using WeatherStation.ImageProcessor.Domain.Interfaces.Facades;
using WeatherStation.ImageProcessor.Domain.Interfaces.Repositories;
using WeatherStation.ImageProcessor.Domain.Interfaces.Services;
using WeatherStation.ImageProcessor.Infrastructure.Models;

namespace WeatherStation.ImageProcessor.Functions.Functions
{
    public class RequestWeatherImagesFunction
    {
        private readonly IJobInitiationFacade _jobInitiationFacade;
        private readonly ILogger<RequestWeatherImagesFunction> _logger;

        public RequestWeatherImagesFunction(
            IJobInitiationFacade jobInitiationFacade,
            ILogger<RequestWeatherImagesFunction> logger)
        {
            _jobInitiationFacade = jobInitiationFacade;
            _logger = logger;
        }

        [Function("RequestWeatherImages")]
        public async Task<HttpResponseData> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("RequestWeatherImages function started processing a request.");

            var jobId = await _jobInitiationFacade.InitiateWeatherJobAsync(cancellationToken);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new { jobId });

            _logger.LogInformation("RequestWeatherImages function completed processing the request.");
            return response;
        }
    }
}
