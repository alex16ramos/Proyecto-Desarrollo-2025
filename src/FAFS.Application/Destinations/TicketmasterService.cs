using FAFS.Application.Contracts.Destinations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace FAFS.Destinations
{
    public class TicketmasterService : ApplicationService, ITicketmasterService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ExternalApisOptions _options;
        private readonly ILogger<TicketmasterService> _logger;

        public TicketmasterService(
            IHttpClientFactory httpClientFactory,
            IOptions<ExternalApisOptions> options,
            ILogger<TicketmasterService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<List<DestinationEventDto>> GetEventsByCityAsync(string city, string? countryCode = null)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                return new List<DestinationEventDto>();
            }

            var apiKey = _options.Ticketmaster.ApiKey;
            var baseUrl = string.IsNullOrWhiteSpace(_options.Ticketmaster.BaseUrl)
                ? "https://app.ticketmaster.com/discovery/v2/"
                : _options.Ticketmaster.BaseUrl.TrimEnd('/') + "/";

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                _logger.LogWarning("Ticketmaster API Key no configurada. Retornando lista vacía de espectáculos.");
                return new List<DestinationEventDto>();
            }

            try
            {
                var startDateTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
                var endDateTime = DateTime.UtcNow.AddMonths(3).ToString("yyyy-MM-ddTHH:mm:ssZ");

                var url = $"{baseUrl}events.json?apikey={Uri.EscapeDataString(apiKey)}" +
                          $"&city={Uri.EscapeDataString(city)}" +
                          $"&startDateTime={Uri.EscapeDataString(startDateTime)}" +
                          $"&endDateTime={Uri.EscapeDataString(endDateTime)}" +
                          $"&size=5&sort=date,asc";

                if (!string.IsNullOrWhiteSpace(countryCode))
                {
                    url += $"&countryCode={Uri.EscapeDataString(countryCode)}";
                }

                var client = _httpClientFactory.CreateClient("TicketmasterClient");
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Llamada a Ticketmaster API falló con código {StatusCode} para la ciudad {City}.", response.StatusCode, city);
                    return new List<DestinationEventDto>();
                }

                using var stream = await response.Content.ReadAsStreamAsync();
                using var doc = await JsonDocument.ParseAsync(stream);

                var events = new List<DestinationEventDto>();

                if (doc.RootElement.TryGetProperty("_embedded", out var embedded) &&
                    embedded.TryGetProperty("events", out var eventsArray) &&
                    eventsArray.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in eventsArray.EnumerateArray())
                    {
                        var eventDto = new DestinationEventDto
                        {
                            Id = item.TryGetProperty("id", out var idProp) ? idProp.GetString() ?? string.Empty : string.Empty,
                            Name = item.TryGetProperty("name", out var nameProp) ? nameProp.GetString() ?? string.Empty : string.Empty,
                            Url = item.TryGetProperty("url", out var urlProp) ? urlProp.GetString() : null,
                            City = city,
                            Country = countryCode
                        };

                        // Fechas
                        if (item.TryGetProperty("dates", out var dates) &&
                            dates.TryGetProperty("start", out var start) &&
                            start.TryGetProperty("dateTime", out var dateTimeProp) &&
                            DateTime.TryParse(dateTimeProp.GetString(), out var parsedDate))
                        {
                            eventDto.EventDate = parsedDate;
                        }

                        // Imagen (priorizar resolución conveniente)
                        if (item.TryGetProperty("images", out var images) && images.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var img in images.EnumerateArray())
                            {
                                if (img.TryGetProperty("url", out var imgUrl))
                                {
                                    eventDto.ImageUrl = imgUrl.GetString();
                                    break;
                                }
                            }
                        }

                        // Recinto / Venue
                        if (item.TryGetProperty("_embedded", out var eventEmbedded) &&
                            eventEmbedded.TryGetProperty("venues", out var venues) &&
                            venues.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var venue in venues.EnumerateArray())
                            {
                                if (venue.TryGetProperty("name", out var venueName))
                                {
                                    eventDto.VenueName = venueName.GetString();
                                    break;
                                }
                            }
                        }

                        // Clasificación / Categoría
                        if (item.TryGetProperty("classifications", out var classifications) &&
                            classifications.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var cls in classifications.EnumerateArray())
                            {
                                if (cls.TryGetProperty("segment", out var segment) &&
                                    segment.TryGetProperty("name", out var segName))
                                {
                                    eventDto.Category = segName.GetString();
                                    break;
                                }
                            }
                        }

                        events.Add(eventDto);
                    }
                }

                return events;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción al consultar eventos de Ticketmaster para {City}.", city);
                return new List<DestinationEventDto>();
            }
        }
    }
}
