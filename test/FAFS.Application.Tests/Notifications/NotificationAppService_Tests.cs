using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FAFS.Application.Contracts.Destinations;
using FAFS.Application.Contracts.Notifications;
using FAFS.Destinations;
using FAFS.Favorites;
using FAFS.Notifications;
using Moq;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace FAFS.Application.Tests.Notifications
{
    public class NotificationAppService_Tests : FAFSApplicationTestBase
    {
        private readonly INotificationAppService _notificationAppService;
        private readonly IRepository<Destination, Guid> _destinationRepository;
        private readonly IRepository<FavoriteDestination, Guid> _favoriteRepository;
        private readonly IRepository<AppNotification, Guid> _notificationRepository;

        public NotificationAppService_Tests()
        {
            _notificationAppService = GetRequiredService<INotificationAppService>();
            _destinationRepository = GetRequiredService<IRepository<Destination, Guid>>();
            _favoriteRepository = GetRequiredService<IRepository<FavoriteDestination, Guid>>();
            _notificationRepository = GetRequiredService<IRepository<AppNotification, Guid>>();
        }

        [Fact]
        public async Task Should_CheckUpcomingEvents_And_Generate_Notifications_For_Favorites()
        {
            var userId = Guid.NewGuid();

            // 1. Crear un destino en la BD
            var destId = Guid.NewGuid();
            var destination = new Destination(
                destId,
                "Madrid Cultural",
                "España",
                "Madrid",
                "http://example.com/madrid.jpg",
                DateTime.UtcNow,
                new Coordinates("40.4168", "-3.7038")
            );
            await _destinationRepository.InsertAsync(destination);

            // 2. Marcar como favorito para este usuario
            var fav = new FavoriteDestination(Guid.NewGuid(), userId, destId);
            await _favoriteRepository.InsertAsync(fav);

            // 3. Mockear el servicio de Ticketmaster
            var mockTicketmaster = new Mock<ITicketmasterService>();
            mockTicketmaster.Setup(s => s.GetEventsByCityAsync("Madrid", "España"))
                .ReturnsAsync(new List<DestinationEventDto>
                {
                    new DestinationEventDto
                    {
                        Id = "TM-MADRID-001",
                        Name = "Concierto Sinfónico Madrid",
                        EventDate = DateTime.UtcNow.AddDays(7),
                        City = "Madrid",
                        Country = "España"
                    }
                });

            // 4. Instanciar NotificationAppService con las dependencias y asignar LazyServiceProvider desde GetRequiredService
            var notificationService = new FAFS.Application.Notifications.NotificationAppService(
                _notificationRepository,
                _favoriteRepository,
                _destinationRepository,
                mockTicketmaster.Object
            )
            {
                LazyServiceProvider = GetRequiredService<Volo.Abp.DependencyInjection.IAbpLazyServiceProvider>()
            };

            // Iniciar sesión como el usuario y ejecutar
            using (Login(userId))
            {
                await notificationService.CheckUpcomingEventsForFavoritesAsync();

                // Verificar que se haya insertado la notificación
                var notifs = await _notificationRepository.GetListAsync(n => n.UserId == userId);
                notifs.ShouldNotBeEmpty();
                notifs.ShouldContain(n => n.Type == "UpcomingEvent" && n.Message.Contains("Concierto Sinfónico Madrid"));

                // Si se ejecuta nuevamente, no debe duplicar la notificación
                await notificationService.CheckUpcomingEventsForFavoritesAsync();
                var notifsAfter = await _notificationRepository.GetListAsync(n => n.UserId == userId && n.Type == "UpcomingEvent");
                notifsAfter.Count.ShouldBe(1);
            }
        }
    }
}
