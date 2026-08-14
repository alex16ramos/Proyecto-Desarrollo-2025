using System;
using Volo.Abp.Application.Dtos;

namespace FAFS.Application.Contracts.Destinations
{
    public class DestinationDto : AuditedEntityDto<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; }
        public string Latitude { get; set; } = string.Empty;
        public string Longitude { get; set; } = string.Empty;
        public double AverageRating { get; set; }
        public int RatingsCount { get; set; }
        public bool IsFavorite { get; set; }
    }
}

