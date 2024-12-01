using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Formats.Jpeg;
using Color = SixLabors.ImageSharp.Color;
using PointF = SixLabors.ImageSharp.PointF;
using WeatherStation.ImageProcessor.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp.PixelFormats;
using WeatherStation.ImageProcessor.Infrastructure.Util;
using WeatherStation.ImageProcessor.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace WeatherStation.ImageProcessor.Infrastructure.Services
{
    public class ImageGenerationService : IImageGenerationService
    {
        private readonly ILogger<ImageGenerationService> _logger;
        private readonly FontCollection _fonts;
        private readonly ImageGenerationOptions _options;

        public ImageGenerationService(ILogger<ImageGenerationService> logger, IOptions<ImageGenerationOptions> options)
        {
            _logger = logger;
            _options = options.Value;
            _fonts = new FontCollection();
            _fonts.AddSystemFonts();
        }

        public Task<Stream> GenerateWeatherImageAsync(
            Stream baseImageStream,
            Domain.Entities.WeatherStation weatherStation,
            string attribution,
            CancellationToken cancellationToken = default) =>
            _logger.ExecuteWithExceptionLoggingAsync<Stream>(
                async () =>
                {
                    using Image image = await Image.LoadAsync(baseImageStream, cancellationToken);

                    ushort overlayHeight = 150;
                    Image<Rgba32> overlay = new(image.Width, overlayHeight);

                    Font font = _fonts.Get(_options.FontName).CreateFont(_options.FontSize);
                    Font smallFont = _fonts.Get(_options.FontName).CreateFont(_options.FontSize * 0.8f);

                    overlay.Mutate(x => x.Fill(Color.ParseHex(_options.OverlayColor)));

                    TextOptions textOptions = new(font)
                    {
                        WordBreaking = WordBreaking.Standard,
                        WrappingLength = image.Width - 40
                    };

                    TextOptions smallTextOptions = new(smallFont)
                    {
                        WordBreaking = WordBreaking.Standard,
                        WrappingLength = image.Width - 40
                    };

                    FontRectangle attributionSize = TextMeasurer.MeasureSize(attribution, smallTextOptions);
                    string weatherText = CreateWeatherText(weatherStation);
                    Color textColor = Color.ParseHex(_options.TextColor);

                    image.Mutate(x => x
                        .DrawImage(overlay, new Point(0, image.Height - overlayHeight), 0.8f)
                        .DrawText(
                            weatherText,
                            font,
                            textColor,
                            new PointF(20, image.Height - overlayHeight + 20))
                        .DrawText(
                            attribution,
                            smallFont,
                            textColor,
                            new PointF(image.Width - attributionSize.Width - 20, image.Height - 40)));

                    Stream outputStream = new MemoryStream();
                    await image.SaveAsJpegAsync(outputStream, new JpegEncoder { Quality = 90 });
                    outputStream.Position = 0;

                    return outputStream;
                },
                "Failed to generate weather image for station {StationId}",
                weatherStation.Id);

        private static string CreateWeatherText(Domain.Entities.WeatherStation weather) =>
            $"{weather.Name} - {weather.Timestamp:HH:mm}\n" +
            $"{weather.WeatherDescription}\n" +
            $"Temperatuur: {weather.Temperature:F1}°C (Voelt als: {weather.FeelTemperature:F1}°C)\n" +
            $"Wind: {weather.WindSpeed:F1} m/s ({weather.WindSpeedBft} Bft) {weather.WindDirection}\n" +
            $"Vochtigheid: {weather.Humidity}%";
    }
}
