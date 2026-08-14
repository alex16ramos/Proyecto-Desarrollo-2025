using System;
using System.Threading.Tasks;
using FAFS.Notifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Volo.Abp.Account;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace FAFS.Users
{
    [Dependency(ReplaceServices = true)]
    [ExposeServices(typeof(IProfileAppService), typeof(ProfileAppService), typeof(MyProfileAppService))]
    public class MyProfileAppService : ProfileAppService
    {
        private readonly IRepository<AppNotification, Guid> _notificationRepository;

        public MyProfileAppService(
            IdentityUserManager userManager,
            IOptions<IdentityOptions> identityOptions,
            IRepository<AppNotification, Guid> notificationRepository)
            : base(userManager, identityOptions)
        {
            _notificationRepository = notificationRepository;
        }

        public override async Task<ProfileDto> UpdateAsync(UpdateProfileDto input)
        {
            var result = await base.UpdateAsync(input);

            if (CurrentUser.IsAuthenticated && CurrentUser.Id.HasValue)
            {
                var notification = new AppNotification(
                    GuidGenerator.Create(),
                    CurrentUser.GetId(),
                    "Actualización de cuenta",
                    "Los datos personales de tu perfil han sido actualizados exitosamente.",
                    "AccountUpdate"
                );
                await _notificationRepository.InsertAsync(notification, autoSave: true);
            }

            return result;
        }

        public override async Task ChangePasswordAsync(ChangePasswordInput input)
        {
            await base.ChangePasswordAsync(input);

            if (CurrentUser.IsAuthenticated && CurrentUser.Id.HasValue)
            {
                var notification = new AppNotification(
                    GuidGenerator.Create(),
                    CurrentUser.GetId(),
                    "Seguridad de la cuenta: Cambio de contraseña",
                    "Tu contraseña de acceso ha sido modificada satisfactoriamente. Si no realizaste esta acción, comunícate con soporte.",
                    "Security"
                );
                await _notificationRepository.InsertAsync(notification, autoSave: true);
            }
        }
    }
}
