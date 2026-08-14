using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FAFS.Application.Contracts.Notifications;
using FAFS.Notifications;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace FAFS.Application.Notifications
{
    [Authorize]
    public class NotificationAppService : ApplicationService, INotificationAppService
    {
        private readonly IRepository<AppNotification, Guid> _notificationRepository;

        public NotificationAppService(IRepository<AppNotification, Guid> notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<List<NotificationDto>> GetUserNotificationsAsync()
        {
            if (!CurrentUser.IsAuthenticated || !CurrentUser.Id.HasValue)
                throw new AbpAuthorizationException("Debe estar autenticado.");

            var userId = CurrentUser.GetId();
            var list = await _notificationRepository.GetListAsync(n => n.UserId == userId);
            var sorted = list.OrderByDescending(n => n.CreationTime).ToList();

            return ObjectMapper.Map<List<AppNotification>, List<NotificationDto>>(sorted);
        }

        public async Task<int> GetUnreadCountAsync()
        {
            if (!CurrentUser.IsAuthenticated || !CurrentUser.Id.HasValue)
                return 0;

            var userId = CurrentUser.GetId();
            var list = await _notificationRepository.GetListAsync(n => n.UserId == userId && !n.IsRead);
            return list.Count;
        }

        public async Task MarkAsReadAsync(Guid id)
        {
            if (!CurrentUser.IsAuthenticated || !CurrentUser.Id.HasValue)
                throw new AbpAuthorizationException("Debe estar autenticado.");

            var notification = await _notificationRepository.GetAsync(id);
            if (notification.UserId != CurrentUser.GetId())
                throw new AbpAuthorizationException("No tiene permisos para modificar esta notificación.");

            notification.MarkAsRead();
            await _notificationRepository.UpdateAsync(notification, autoSave: true);
        }

        public async Task MarkAllAsReadAsync()
        {
            if (!CurrentUser.IsAuthenticated || !CurrentUser.Id.HasValue)
                throw new AbpAuthorizationException("Debe estar autenticado.");

            var userId = CurrentUser.GetId();
            var unreadNotifications = await _notificationRepository.GetListAsync(n => n.UserId == userId && !n.IsRead);

            foreach (var item in unreadNotifications)
            {
                item.MarkAsRead();
                await _notificationRepository.UpdateAsync(item, autoSave: false);
            }
        }

        public async Task<NotificationDto> SendNotificationAsync(CreateNotificationDto input)
        {
            var notification = new AppNotification(
                GuidGenerator.Create(),
                input.UserId,
                input.Title,
                input.Message,
                input.Type ?? "info"
            );

            await _notificationRepository.InsertAsync(notification, autoSave: true);
            return ObjectMapper.Map<AppNotification, NotificationDto>(notification);
        }
    }
}
