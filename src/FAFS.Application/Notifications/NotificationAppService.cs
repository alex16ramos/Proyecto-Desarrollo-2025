using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FAFS.Application.Contracts.Destinations;
using FAFS.Application.Contracts.Notifications;
using FAFS.Destinations;
using FAFS.Favorites;
using FAFS.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
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
        private readonly IRepository<FavoriteDestination, Guid>? _favoriteRepository;
        private readonly IRepository<Destination, Guid>? _destinationRepository;
        private readonly ITicketmasterService? _ticketmasterService;

        public NotificationAppService(
            IRepository<AppNotification, Guid> notificationRepository,
            IRepository<FavoriteDestination, Guid>? favoriteRepository = null,
            IRepository<Destination, Guid>? destinationRepository = null,
            ITicketmasterService? ticketmasterService = null)
        {
            _notificationRepository = notificationRepository;
            _favoriteRepository = favoriteRepository;
            _destinationRepository = destinationRepository;
            _ticketmasterService = ticketmasterService;
        }

        public async Task<List<NotificationDto>> GetUserNotificationsAsync()
        {
            if (!CurrentUser.IsAuthenticated || !CurrentUser.Id.HasValue)
                throw new AbpAuthorizationException("Debe estar autenticado.");

            // Al consultar las notificaciones, verificamos si hay eventos próximos para sus favoritos
            await CheckUpcomingEventsForFavoritesAsync();

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

        public async Task CheckUpcomingEventsForFavoritesAsync()
        {
            if (!CurrentUser.IsAuthenticated || !CurrentUser.Id.HasValue)
                return;

            if (_favoriteRepository == null || _destinationRepository == null || _ticketmasterService == null)
                return;

            var userId = CurrentUser.GetId();
            var userFavorites = await _favoriteRepository.GetListAsync(f => f.UserId == userId);
            if (!userFavorites.Any())
                return;

            var destinationIds = userFavorites.Select(f => f.DestinationId).Distinct().ToList();
            var destinations = await _destinationRepository.GetListAsync(d => destinationIds.Contains(d.Id));

            // Notificaciones existentes del usuario para evitar duplicar alertas sobre el mismo evento
            var existingNotifications = await _notificationRepository.GetListAsync(n => n.UserId == userId && n.Type == "UpcomingEvent");

            foreach (var dest in destinations)
            {
                if (string.IsNullOrWhiteSpace(dest.City))
                    continue;

                try
                {
                    var events = await _ticketmasterService.GetEventsByCityAsync(dest.City, dest.Country);
                    if (events == null || !events.Any())
                        continue;

                    // Tomamos los primeros 2 eventos más próximos
                    foreach (var evt in events.Take(2))
                    {
                        var eventTitle = $"Próximo evento en {dest.Name}";
                        var dateStr = evt.EventDate.HasValue ? evt.EventDate.Value.ToString("dd/MM/yyyy") : "próximamente";
                        var eventMsg = $"'{evt.Name}' se llevará a cabo en {dest.City} ({dateStr}).";

                        // Si ya se notificó al usuario este evento en específico, no volvemos a generar notificación
                        var alreadyNotified = existingNotifications.Any(n => 
                            n.Message.Contains(evt.Name, StringComparison.OrdinalIgnoreCase) || 
                            (!string.IsNullOrEmpty(evt.Id) && n.Message.Contains(evt.Id, StringComparison.OrdinalIgnoreCase)));

                        if (!alreadyNotified)
                        {
                            var notification = new AppNotification(
                                GuidGenerator.Create(),
                                userId,
                                eventTitle,
                                $"{eventMsg} (Ref: {evt.Id})",
                                "UpcomingEvent"
                            );

                            await _notificationRepository.InsertAsync(notification, autoSave: true);
                            existingNotifications.Add(notification);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex, "Error al comprobar eventos para destino favorito {DestinationName}", dest.Name);
                }
            }
        }
    }
}
