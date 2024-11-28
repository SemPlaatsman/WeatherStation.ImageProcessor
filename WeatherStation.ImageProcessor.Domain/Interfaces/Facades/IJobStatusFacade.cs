using WeatherStation.ImageProcessor.Domain.DTOs.Response;

namespace WeatherStation.ImageProcessor.Domain.Interfaces.Facades
{
    public interface IJobStatusFacade
    {
        Task<JobStatusResponse> GetJobStatusAsync(string jobId, CancellationToken cancellationToken = default);
    }
}
