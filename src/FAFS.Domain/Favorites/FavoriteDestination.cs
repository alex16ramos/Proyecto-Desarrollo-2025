using System;
using Volo.Abp.Domain.Entities;
using FAFS;

namespace FAFS.Favorites
{
    public class FavoriteDestination : AggregateRoot<Guid>, IUserOwned
    {
        public Guid UserId { get; set; }
        public Guid DestinationId { get; set; }
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;

        protected FavoriteDestination() { }

        public FavoriteDestination(Guid id, Guid userId, Guid destinationId)
            : base(id)
        {
            UserId = userId;
            DestinationId = destinationId;
            CreationTime = DateTime.UtcNow;
        }
    }
}
