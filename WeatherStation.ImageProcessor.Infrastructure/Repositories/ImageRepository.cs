using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using WeatherStation.ImageProcessor.Domain.Interfaces.Repositories;
using WeatherStation.ImageProcessor.Infrastructure.Options;

namespace WeatherStation.ImageProcessor.Infrastructure.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly BlobContainerClient _containerClient;

        public ImageRepository(IOptions<StorageOptions> options)
        {
            var blobServiceClient = new BlobServiceClient(options.Value.BlobStorageConnection);
            _containerClient = blobServiceClient.GetBlobContainerClient(options.Value.ImageContainerName);
            _containerClient.CreateIfNotExists(PublicAccessType.Blob);
        }

        public async Task SaveImageAsync(
            string jobId,
            string stationId,
            Stream imageStream,
            CancellationToken cancellationToken = default)
        {
            var blobClient = _containerClient.GetBlobClient($"{jobId}/{stationId}.jpg");
            await blobClient.UploadAsync(imageStream, overwrite: true, cancellationToken);
        }

        public async Task<IEnumerable<(string StationId, string Url)>> GetImagesForJobAsync(
            string jobId, CancellationToken cancellationToken = default)
        {
            var images = new List<(string StationId, string Url)>();
            var prefix = $"{jobId}/";

            await foreach (var blob in _containerClient.GetBlobsAsync(prefix: prefix, cancellationToken: cancellationToken))
            {
                var stationId = Path.GetFileNameWithoutExtension(blob.Name.Substring(prefix.Length));
                var blobClient = _containerClient.GetBlobClient(blob.Name);
                images.Add((stationId, blobClient.Uri.ToString()));
            }

            return images;
        }
    }
}
