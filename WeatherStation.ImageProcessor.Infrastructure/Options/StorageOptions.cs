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

        // Queue storage
        public string QueueStorageConnection { get; set; } = default!;
        public string InitiationQueueName { get; set; } = "initiation-queue";
        public string ProcessingQueueName { get; set; } = "processing-queue";
    }
}
