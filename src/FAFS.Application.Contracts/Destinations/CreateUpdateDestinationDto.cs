using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;


namespace FAFS.Application.Contracts.Destinations
{
    public class CreateUpdateDestinationDto : AuditedEntityDto<Guid>
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(200, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false)]
        [StringLength(100, MinimumLength = 1)]
        public string Country { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public string Latitude { get; set; } = string.Empty;
        public string Longitude { get; set; } = string.Empty;
    }
}