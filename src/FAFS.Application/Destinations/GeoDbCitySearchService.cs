using FAFS.Application.Contracts.Destinations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp;

namespace FAFS.Destinations
{
    // Service that connects to the external GeoDB Cities API
    public class GeoDbCitySearchService : ICitySearchService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ExternalApisOptions _options;
        private readonly ILogger<GeoDbCitySearchService>? _logger;

        public GeoDbCitySearchService(
            IHttpClientFactory httpClientFactory,
            IOptions<ExternalApisOptions> options,
            ILogger<GeoDbCitySearchService>? logger = null)
        {
            _httpClientFactory = httpClientFactory;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<CitySearchResultDto> SearchCitiesAsync(CitySearchRequestDto request)
        {
            // Validación básica
            if (string.IsNullOrWhiteSpace(request.PartialName) || request.PartialName.Trim().Length < 2)
            {
                throw new BusinessException("CitySearch:InvalidPartialName")
                    .WithData("Message", "The search text must contain at least 2 characters.");
            }

            var client = _httpClientFactory.CreateClient("GeoDbClient");
            var baseUrl = !string.IsNullOrWhiteSpace(_options.GeoDb.BaseUrl)
                ? _options.GeoDb.BaseUrl.TrimEnd('/') + "/"
                : "https://wft-geo-db.p.rapidapi.com/v1/geo/";

            var limit = request.Limit > 0 ? Math.Min(request.Limit, 10) : 5;
            var url = $"{baseUrl}cities?namePrefix={Uri.EscapeDataString(request.PartialName.Trim())}&limit={limit}&sort=-population";

            if (!string.IsNullOrWhiteSpace(request.CountryCode))
            {
                url += $"&countryIds={Uri.EscapeDataString(request.CountryCode.Trim())}";
            }

            const int maxRetries = 3;
            HttpResponseMessage? response = null;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    using var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                    if (!string.IsNullOrWhiteSpace(_options.GeoDb.ApiKey))
                    {
                        requestMessage.Headers.TryAddWithoutValidation("X-RapidAPI-Key", _options.GeoDb.ApiKey.Trim());
                    }
                    if (!string.IsNullOrWhiteSpace(_options.GeoDb.ApiHost))
                    {
                        requestMessage.Headers.TryAddWithoutValidation("X-RapidAPI-Host", _options.GeoDb.ApiHost.Trim());
                    }

                    response = await client.SendAsync(requestMessage);

                    if (response.StatusCode == HttpStatusCode.TooManyRequests) // 429 Rate Limit
                    {
                        _logger?.LogWarning("GeoDB rate limit alcanzado (429). Reintento {Attempt}/{MaxRetries} tras espera de 1.2s.", attempt, maxRetries);
                        if (attempt < maxRetries)
                        {
                            await Task.Delay(1200); // 1.2s delay as specified in technical documentation
                            continue;
                        }
                    }

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        _logger?.LogError("Error al consultar GeoDB. Status: {StatusCode}, Body: {Body}", (int)response.StatusCode, errorContent);
                    }

                    response.EnsureSuccessStatusCode();
                    break;
                }
                catch (HttpRequestException ex) when (attempt < maxRetries)
                {
                    _logger?.LogWarning(ex, "Fallo de conexión con GeoDB en intento {Attempt}. Reintentando...", attempt);
                    await Task.Delay(1200);
                }
            }

            if (response == null || !response.IsSuccessStatusCode)
            {
                return new CitySearchResultDto();
            }

            try
            {
                using var stream = await response.Content.ReadAsStreamAsync();
                using var doc = await JsonDocument.ParseAsync(stream);

                var result = new CitySearchResultDto();

                if (doc.RootElement.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in data.EnumerateArray())
                    {
                        result.Cities.Add(new CityDto
                        {
                            Name = item.GetProperty("name").GetString() ?? string.Empty,
                            Country = item.GetProperty("country").GetString() ?? string.Empty,
                            CountryCode = item.GetProperty("countryCode").GetString() ?? string.Empty,
                            Region = item.TryGetProperty("region", out var r) ? r.GetString() : null,
                            Latitude = item.TryGetProperty("latitude", out var lat) ? lat.ToString() : null,
                            Longitude = item.TryGetProperty("longitude", out var lon) ? lon.ToString() : null
                        });
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error al deserializar respuesta de GeoDB.");
                throw;
            }
        }
    }

    // External API configuration 
    public class ExternalApisOptions
    {
        public GeoDbOptions GeoDb { get; set; } = new();
        public TicketmasterOptions Ticketmaster { get; set; } = new();

        public class GeoDbOptions
        {
            public string BaseUrl { get; set; } = "https://wft-geo-db.p.rapidapi.com/v1/geo/";
            public string ApiHost { get; set; } = "wft-geo-db.p.rapidapi.com";
            public string ApiKey { get; set; } = string.Empty;
        }

        public class TicketmasterOptions
        {
            public string BaseUrl { get; set; } = "https://app.ticketmaster.com/discovery/v2/";
            public string ApiKey { get; set; } = string.Empty;
        }
    }
}