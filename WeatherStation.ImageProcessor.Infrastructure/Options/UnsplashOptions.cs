namespace WeatherStation.ImageProcessor.Infrastructure.Options
{
    public record UnsplashOptions
    {
        public const string SectionName = "External:Unsplash";
        public required string BaseUrl { get; init; } = "https://api.unsplash.com/photos/random";
        public required string AccessKey { get; init; }
        public required string Orientation { get; init; } = "landscape";
        public required int TimeoutSeconds { get; init; } = 30;
        public required int RetryCount { get; init; } = 3;
    }
}
