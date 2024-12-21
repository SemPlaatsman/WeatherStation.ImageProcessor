using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;
using WeatherStation.ImageProcessor.Domain.Interfaces.Repositories;
using WeatherStation.ImageProcessor.Infrastructure.Options;

namespace WeatherStation.ImageProcessor.Infrastructure.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly BlobContainerClient _containerClient;
        private readonly StorageOptions _options;

        public ImageRepository(IOptions<StorageOptions> options)
        {
            _options = options.Value;
            BlobServiceClient blobServiceClient = new(options.Value.BlobStorageConnection);
            _containerClient = blobServiceClient.GetBlobContainerClient(options.Value.ImageContainerName);
            _containerClient.CreateIfNotExists(PublicAccessType.None);
        }

        public async Task SaveImageAsync(
            string jobId,
            string stationId,
            Stream imageStream,
            CancellationToken cancellationToken = default)
        {
            BlobClient blobClient = _containerClient.GetBlobClient($"{jobId}/{stationId}.jpg");
            await blobClient.UploadAsync(imageStream, overwrite: true, cancellationToken);
        }

        public async Task<IEnumerable<(string StationId, string Url)>> GetImagesForJobAsync(
            string jobId, CancellationToken cancellationToken = default)
        {
            List<(string StationId, string Url)> images = new();
            string prefix = $"{jobId}/";

            await foreach (BlobItem blob in _containerClient.GetBlobsAsync(prefix: prefix, cancellationToken: cancellationToken))
            {
                string? stationId = Path.GetFileNameWithoutExtension(blob.Name.Substring(prefix.Length));
                string sasUrl = GenerateSasUrl(blob.Name);
                images.Add((stationId, sasUrl));
            }

            return images;
        }

        public string GenerateSasUrl(string blobPath)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(blobPath);
            var expiresOn = DateTimeOffset.UtcNow.AddHours(_options.SasTokenExpiryHours);

            // Create SAS token that's valid for X hours
            BlobSasBuilder sasBuilder = new()
            {
                BlobContainerName = _containerClient.Name,
                BlobName = blobPath,
                Resource = "b", // b=blob
                StartsOn = DateTimeOffset.UtcNow.Subtract(TimeSpan.FromMinutes(5)), // Start 5 minutes ago to avoid clock skew issues
                ExpiresOn = expiresOn
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            // Generate the SAS token
            string sasToken = sasBuilder.ToSasQueryParameters(
                new Azure.Storage.StorageSharedKeyCredential(
                    _options.GetStorageAccountName(),
                    _options.GetStorageAccountKey())
            ).ToString();


            return $"{blobClient.Uri}?{sasToken}";
        }
    }
}
