using System.Text.Json.Serialization;

namespace WeatherStation.ImageProcessor.Infrastructure.External.Models.Buienradar
{
    internal record BuienradarResponse
    {
        public Actual? Actual { get; init; }
    }

    internal record Actual
    {
        [JsonPropertyName("stationmeasurements")]
        public List<StationMeasurement>? StationMeasurements { get; init; }
    }

    internal record StationMeasurement
    {
        [JsonPropertyName("stationid")]
        public int StationId { get; init; }

        [JsonPropertyName("stationname")]
        public required string StationName { get; init; }

        [JsonPropertyName("regio")]
        public string? Region { get; init; }

        [JsonPropertyName("lat")]
        public double Latitude { get; init; }

        [JsonPropertyName("lon")]
        public double Longitude { get; init; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; init; }

        [JsonPropertyName("weatherdescription")]
        public required string WeatherDescription { get; init; }

        [JsonPropertyName("iconurl")]
        public required string IconUrl { get; init; }

        [JsonPropertyName("temperature")]
        public double? Temperature { get; init; }

        [JsonPropertyName("feeltemperature")]
        public double? FeelTemperature { get; init; }

        [JsonPropertyName("windspeed")]
        public double? WindSpeed { get; init; }

        [JsonPropertyName("windspeedBft")]
        public int? WindSpeedBft { get; init; }

        [JsonPropertyName("winddirection")]
        public string? WindDirection { get; init; }

        [JsonPropertyName("humidity")]
        public double? Humidity { get; init; }

        [JsonPropertyName("precipitation")]
        public double? Precipitation { get; init; }

        [JsonPropertyName("rainFallLastHour")]
        public double? RainFallLastHour { get; init; }

        [JsonPropertyName("visibility")]
        public double? Visibility { get; init; }
    }
}