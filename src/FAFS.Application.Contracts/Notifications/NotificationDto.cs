using System;
using Volo.Abp.Application.Dtos;

namespace FAFS.Application.Contracts.Notifications
{
    public class NotificationDto : EntityDto<Guid>
    {
        public Guid UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Type { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreationTime { get; set; }
    }

    public class CreateNotificationDto
    {
        public Guid UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Type { get; set; } = "info";
    }
}
