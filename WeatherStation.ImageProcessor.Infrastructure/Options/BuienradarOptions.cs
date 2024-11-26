namespace WeatherStation.ImageProcessor.Infrastructure.Options
{
    public record BuienradarOptions
    {
        public const string SectionName = "External:Buienradar";
        public required string Endpoint { get; init; } = "https://data.buienradar.nl/2.0/feed/json";
        public required int TimeoutSeconds { get; init; } = 30;
        public required int RetryCount { get; init; } = 3;
    }
}
