namespace WeatherStation.ImageProcessor.Domain.Interfaces.Repositories
{
    public interface IImageRepository
    {
        Task SaveImageAsync(
            string jobId,
            string stationId,
            Stream imageStream,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<(string StationId, string Url)>> GetImagesForJobAsync(
            string jobId, CancellationToken cancellationToken = default);

        string GenerateSasUrl(string blobPath);
    }
}
