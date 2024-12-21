namespace WeatherStation.ImageProcessor.Infrastructure.Options
{
    public class StorageOptions
    {
        public const string SectionName = "Storage";

        // Table storage
        public string TableStorageConnection { get; set; } = default!;
        public string JobTableName { get; set; } = "WeatherJobs";

        // Blob storage
        public string BlobStorageConnection { get; set; } = default!;
        public string ImageContainerName { get; set; } = "weather-images";
        public int SasTokenExpiryHours { get; set; } = 24;
        public string BlobStorageAccountName { get; set; } = default!;
        public string BlobStorageAccountKey { get; set; } = default!;

        // Queue storage
        public string QueueStorageConnection { get; set; } = default!;
        public string InitiationQueueName { get; set; } = "initiation-queue";
        public string ProcessingQueueName { get; set; } = "processing-queue";

        // Helpers for SAS in development
        private const string DevStorageAccountName = "devstoreaccount1"; // default development storage account name
        private const string DevStorageAccountKey = "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw=="; // default development storage account key

        // Helper properties for SAS generation
        public bool IsDevStorage => BlobStorageConnection.Contains("UseDevelopmentStorage=true");
        public string GetStorageAccountName() => IsDevStorage ? DevStorageAccountName : BlobStorageAccountName;
        public string GetStorageAccountKey() => IsDevStorage ? DevStorageAccountKey : BlobStorageAccountKey;
    }
}
