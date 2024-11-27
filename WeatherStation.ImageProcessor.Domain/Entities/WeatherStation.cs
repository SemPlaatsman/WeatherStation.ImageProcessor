namespace WeatherStation.ImageProcessor.Domain.Entities
{
    public record WeatherStation(
        // Station Information
        int Id,
        string Name,
        string Region,
        double Latitude,
        double Longitude,

        // Weather Data
        DateTime Timestamp,
        string WeatherDescription,
        string IconUrl,
        double Temperature,
        double FeelTemperature,
        double WindSpeed,
        int WindSpeedBft,
        string WindDirection,
        double Humidity,
        double? Precipitation,
        double? RainFallLastHour,
        double? Visibility
    );
}
