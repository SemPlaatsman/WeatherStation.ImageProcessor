using System.ComponentModel.DataAnnotations;

namespace WeatherStation.ImageProcessor.Domain.DTOs.Request
{
    public record WeatherImagesRequest
    {
        [Range(1, 100, ErrorMessage = "When specified, number of stations must be between 1 and 100")]
        public int? NumberOfStations { get; init; }
    }
}