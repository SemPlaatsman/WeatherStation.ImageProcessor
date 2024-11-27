namespace WeatherStation.ImageProcessor.Infrastructure.Models
{
    public record ProcessWeatherImageMessage
    {
        public required string JobId { get; init; }
        public required Domain.Entities.WeatherStation WeatherData { get; init; }
    }
}