namespace WeatherStation.ImageProcessor.Domain.Entities
{
    public record WeatherImageMetadata
    {
        public required string Id { get; init; }
        public string? Description { get; init; }
        public required string Author { get; init; }
        public required string Attribution { get; init; }
    }
}
