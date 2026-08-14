using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace FAFS.Application.Contracts.Experiences
{
    public class ExperienceDto : FullAuditedEntityDto<Guid>
    {
        public Guid DestinationId { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DateExperienced { get; set; }
        public int Rating { get; set; }
        public string? Comments { get; set; }
        public string? DestinationName { get; set; }
    }

    public class CreateUpdateExperienceDto
    {
        [Required]
        public Guid DestinationId { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(200, MinimumLength = 2)]
        public string Title { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false)]
        [StringLength(4000, MinimumLength = 5)]
        public string Description { get; set; } = string.Empty;

        public DateTime DateExperienced { get; set; } = DateTime.UtcNow;

        [Range(1, 5)]
        public int Rating { get; set; } = 5;

        [StringLength(2000)]
        public string? Comments { get; set; }
    }

    public class GetExperiencesInput : PagedAndSortedResultRequestDto
    {
        public Guid? DestinationId { get; set; }
        public Guid? UserId { get; set; }
        public string? Filter { get; set; }
        public int? MinRating { get; set; }
        public int? MaxRating { get; set; }
    }
}
