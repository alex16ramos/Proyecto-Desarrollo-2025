using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace FAFS.Application.Contracts.Notifications
{
    public interface INotificationAppService : IApplicationService
    {
        Task<List<NotificationDto>> GetUserNotificationsAsync();
        Task<int> GetUnreadCountAsync();
        Task MarkAsReadAsync(Guid id);
        Task MarkAllAsReadAsync();
        Task<NotificationDto> SendNotificationAsync(CreateNotificationDto input);
        Task CheckUpcomingEventsForFavoritesAsync();
    }
}
