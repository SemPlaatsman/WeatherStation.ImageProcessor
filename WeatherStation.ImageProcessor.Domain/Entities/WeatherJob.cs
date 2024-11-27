namespace WeatherStation.ImageProcessor.Domain.Entities
{
    public class WeatherJob
    {
        public string Id { get; set; } = default!;
        public string Status { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public int CompletedImages { get; set; }
        public int? TotalImages { get; set; }
        public int? RequestedStations { get; set; }
    }
}
