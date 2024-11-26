﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;
using WeatherStation.ImageProcessor.Domain.Entities;
using WeatherStation.ImageProcessor.Domain.Interfaces.Clients;
using WeatherStation.ImageProcessor.Infrastructure.External.Models.Unsplash;
using WeatherStation.ImageProcessor.Infrastructure.Options;

namespace WeatherStation.ImageProcessor.Infrastructure.Clients
{
    public class UnsplashClient : IImageClient
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<UnsplashOptions> _options;
        private readonly ILogger<UnsplashClient> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public UnsplashClient(
            HttpClient httpClient,
            IOptions<UnsplashOptions> options,
            ILogger<UnsplashClient> logger)
        {
            _httpClient = httpClient;
            _options = options;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<WeatherImage> GetRandomImageAsync(
            string orientation,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var queryParams = new Dictionary<string, string>
                {
                    ["client_id"] = _options.Value.AccessKey,
                    ["orientation"] = orientation,
                    ["query"] = "weather landscape",
                    ["content_filter"] = "high"
                };

                var queryString = string.Join("&", queryParams.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));
                var requestUrl = $"{_options.Value.BaseUrl}?{queryString}";

                var response = await _httpClient.GetFromJsonAsync<UnsplashResponse>(
                    requestUrl,
                    _jsonOptions,
                    cancellationToken);

                if (response == null)
                {
                    throw new Exception("Invalid response from Unsplash API");
                }

                // Register download
                using var downloadRequest = new HttpRequestMessage(HttpMethod.Get, response.Links.DownloadLocation);
                downloadRequest.Headers.Add("client_id", _options.Value.AccessKey);
                await _httpClient.SendAsync(downloadRequest, cancellationToken);

                return new WeatherImage
                {
                    ImageUrl = response.Urls.Regular,
                    Metadata = new WeatherImageMetadata
                    {
                        Id = response.Id,
                        Description = response.Description,
                        Author = response.User.Name,
                        Attribution = $"Photo by {response.User.Name} on Unsplash"
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch image from Unsplash");
                throw;
            }
        }

        public async Task<Stream> DownloadImageAsync(
            string url,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await _httpClient.GetStreamAsync(url, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to download image from URL: {Url}", url);
                throw;
            }
        }
    }
}