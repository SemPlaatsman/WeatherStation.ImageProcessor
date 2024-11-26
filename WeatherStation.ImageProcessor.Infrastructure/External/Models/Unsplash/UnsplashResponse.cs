using System.Text.Json.Serialization;

namespace WeatherStation.ImageProcessor.Infrastructure.External.Models.Unsplash
{
    internal record UnsplashResponse
    {
        [JsonPropertyName("id")]
        public required string Id { get; init; }

        [JsonPropertyName("description")]
        public string? Description { get; init; }

        [JsonPropertyName("urls")]
        public required UnsplashUrls Urls { get; init; }

        [JsonPropertyName("user")]
        public required UnsplashUser User { get; init; }

        [JsonPropertyName("links")]
        public required UnsplashLinks Links { get; init; }
    }

    internal record UnsplashUrls
    {
        [JsonPropertyName("raw")]
        public required string Raw { get; init; }

        [JsonPropertyName("full")]
        public required string Full { get; init; }

        [JsonPropertyName("regular")]
        public required string Regular { get; init; }

        [JsonPropertyName("small")]
        public required string Small { get; init; }

        [JsonPropertyName("thumb")]
        public required string Thumb { get; init; }
    }

    internal record UnsplashLinks
    {
        [JsonPropertyName("download")]
        public required string Download { get; init; }

        [JsonPropertyName("download_location")]
        public required string DownloadLocation { get; init; }
    }

    internal record UnsplashUser
    {
        [JsonPropertyName("username")]
        public required string Username { get; init; }

        [JsonPropertyName("name")]
        public required string Name { get; init; }
    }
}