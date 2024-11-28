using WeatherStation.ImageProcessor.Domain.DTOs.Common;

namespace WeatherStation.ImageProcessor.Domain.DTOs.Response
{
    public class JobStatusResponse
    {
        public string Status { get; set; } = null!;
        public ProgressInfo? Progress { get; set; }
        public IEnumerable<ImageInfo>? Images { get; set; }
    }
}
