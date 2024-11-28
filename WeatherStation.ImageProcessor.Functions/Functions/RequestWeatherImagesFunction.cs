using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using WeatherStation.ImageProcessor.Domain.DTOs.Request;
using WeatherStation.ImageProcessor.Domain.Interfaces.Facades;

namespace WeatherStation.ImageProcessor.Functions.Functions
{
    public class RequestWeatherImagesFunction
    {
        private readonly IJobInitiationFacade _jobInitiationFacade;
        private readonly ILogger<RequestWeatherImagesFunction> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public RequestWeatherImagesFunction(
            IJobInitiationFacade jobInitiationFacade,
            ILogger<RequestWeatherImagesFunction> logger)
        {
            _jobInitiationFacade = jobInitiationFacade;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        [Function("RequestWeatherImages")]
        public async Task<HttpResponseData> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("RequestWeatherImages function started processing a request.");

            WeatherImagesRequest? request;
            try
            {
                request = await JsonSerializer.DeserializeAsync<WeatherImagesRequest>(
                    req.Body,
                    _jsonOptions,
                    cancellationToken);
            }
            catch (JsonException)
            {
                request = null;
            }

            request ??= new WeatherImagesRequest();

            if (request.NumberOfStations.HasValue)
            {
                var validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(request, new ValidationContext(request), validationResults, true))
                {
                    var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await errorResponse.WriteAsJsonAsync(new { errors = validationResults });
                    return errorResponse;
                }
            }

            _logger.LogInformation("Requesting {numberOfStations} stations.",
                request.NumberOfStations.HasValue ? request.NumberOfStations.Value : "all");

            var jobId = await _jobInitiationFacade.InitiateWeatherJobAsync(
                request.NumberOfStations,
                cancellationToken);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new { jobId });

            _logger.LogInformation("RequestWeatherImages function completed processing the request.");
            return response;
        }
    }
}
