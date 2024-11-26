namespace WeatherStation.ImageProcessor.Domain.Entities
{
    public record WeatherStation(
        // Station Information
        int StationId,
        string StationName,
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
