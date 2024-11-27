namespace WeatherStation.ImageProcessor.Infrastructure.Options
{
    public class ImageGenerationOptions
    {
        public const string SectionName = "ImageGeneration";
        public string FontName { get; set; } = "Arial";
        public float FontSize { get; set; } = 24;
        public string TextColor { get; set; } = "#FFFFFF";
        public string OverlayColor { get; set; } = "#00000080";
    }
}
