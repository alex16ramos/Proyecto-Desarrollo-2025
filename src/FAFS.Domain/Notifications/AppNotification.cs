using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Users;

namespace FAFS.Notifications
{
    public class AppNotification : AggregateRoot<Guid>, IUserOwned
    {
        public Guid UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Type { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreationTime { get; set; }

        protected AppNotification()
        {
        }

        public AppNotification(
            Guid id,
            Guid userId,
            string title,
            string message,
            string? type = "info")
            : base(id)
        {
            UserId = userId;
            Title = title;
            Message = message;
            Type = type;
            IsRead = false;
            CreationTime = DateTime.UtcNow;
        }

        public void MarkAsRead()
        {
            IsRead = true;
        }

        public void MarkAsUnread()
        {
            IsRead = false;
        }
    }
}
