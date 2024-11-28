using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using WeatherStation.ImageProcessor.Domain.Interfaces.Facades;

namespace WeatherStation.ImageProcessor.Functions.Functions
{
    public class GetJobStatusFunction
    {
        private readonly IJobStatusFacade _jobStatusFacade;
        private readonly ILogger<GetJobStatusFunction> _logger;

        public GetJobStatusFunction(
            IJobStatusFacade jobStatusFacade,
            ILogger<GetJobStatusFunction> logger)
        {
            _jobStatusFacade = jobStatusFacade;
            _logger = logger;
        }

        [Function("GetJobStatus")]
        public async Task<HttpResponseData> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "jobs/{jobId}/status")] HttpRequestData req,
            string jobId,
            CancellationToken cancellationToken)
        {
            try
            {
                var status = await _jobStatusFacade.GetJobStatusAsync(jobId, cancellationToken);
                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(status, cancellationToken);
                return response;
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Job not found: {JobId}", jobId);
                var response = req.CreateResponse(HttpStatusCode.NotFound);
                await response.WriteStringAsync($"Job with ID {jobId} not found");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting job status for {JobId}", jobId);
                var response = req.CreateResponse(HttpStatusCode.InternalServerError);
                await response.WriteStringAsync("An error occurred while processing your request");
                return response;
            }
        }
    }
}
