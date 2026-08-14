using System;
using Volo.Abp.Domain.Entities.Auditing;
using FAFS;

namespace FAFS.Experiences
{
    public class Experience : FullAuditedAggregateRoot<Guid>, IUserOwned
    {
        public Guid DestinationId { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DateExperienced { get; set; } = DateTime.UtcNow;
        public int Rating { get; set; }
        public string? Comments { get; set; }

        protected Experience() { }

        public Experience(
            Guid id,
            Guid destinationId,
            Guid userId,
            string title,
            string description,
            DateTime dateExperienced,
            int rating = 5,
            string? comments = null
        ) : base(id)
        {
            DestinationId = destinationId;
            UserId = userId;
            Title = title;
            Description = description;
            DateExperienced = dateExperienced;
            Rating = rating;
            Comments = comments;
        }
    }
}
