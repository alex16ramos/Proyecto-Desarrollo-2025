using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace FAFS.Application.Contracts.Users
{
    public interface IUserProfileAppService : IApplicationService
    {
        Task<PublicUserProfileDto> GetPublicProfileAsync(Guid userId);
        Task<PublicUserProfileDto> GetMyProfileAsync();
        Task UpdatePreferencesAsync(UpdateUserPreferencesDto input);
        Task DeleteAccountAsync();
    }
}
