using WeatherStation.ImageProcessor.Domain.Entities;

namespace WeatherStation.ImageProcessor.Domain.Interfaces.Repositories
{
    public interface IJobRepository
    {
        Task<WeatherJob> CreateJobAsync(WeatherJob job, CancellationToken cancellationToken = default);
        Task<WeatherJob?> GetJobAsync(string jobId, CancellationToken cancellationToken = default);
        Task UpdateJobAsync(WeatherJob job, CancellationToken cancellationToken = default);
    }
}
