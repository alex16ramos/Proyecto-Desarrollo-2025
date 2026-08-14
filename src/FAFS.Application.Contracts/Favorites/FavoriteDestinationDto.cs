using System;
using Volo.Abp.Application.Dtos;
using FAFS.Application.Contracts.Destinations;

namespace FAFS.Application.Contracts.Favorites
{
    public class FavoriteDestinationDto : EntityDto<Guid>
    {
        public Guid UserId { get; set; }
        public Guid DestinationId { get; set; }
        public DateTime CreationTime { get; set; }
        public DestinationDto? Destination { get; set; }
    }

    public class ToggleFavoriteDto
    {
        public Guid DestinationId { get; set; }
    }
}
