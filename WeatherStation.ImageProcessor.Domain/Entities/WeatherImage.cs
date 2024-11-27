namespace WeatherStation.ImageProcessor.Domain.Entities
{
    public record WeatherImage
    {
        public required string Url { get; init; }
        public required WeatherImageMetadata Metadata { get; init; }
    }
}
