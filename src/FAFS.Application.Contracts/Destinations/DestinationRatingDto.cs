using System;
using Volo.Abp.Application.Dtos;

namespace FAFS.Application.Contracts.Destinations
{
    public class DestinationRatingDto : EntityDto<Guid>
    {
        public Guid UserId { get; set; }
        public Guid DestinationId { get; set; }
        public int Score { get; set; }
        public string? Comment { get; set; }
        public DateTime CreationTime { get; set; }
    }

    public class RateDestinationDto
    {
        public Guid DestinationId { get; set; }
        public int Score { get; set; }
        public string? Comment { get; set; }
    }

    public class DestinationRatingSummaryDto
    {
        public Guid DestinationId { get; set; }
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
    }
}
