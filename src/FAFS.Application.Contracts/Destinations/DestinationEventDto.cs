using System;

namespace FAFS.Application.Contracts.Destinations
{
    public class DestinationEventDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? EventDate { get; set; }
        public string? VenueName { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Url { get; set; }
        public string? ImageUrl { get; set; }
        public string? Category { get; set; }
    }
}
