using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FAFS.Application.Contracts.Experiences;
using FAFS.Application.Contracts.Users;
using FAFS.Experiences;
using FAFS.Favorites;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace FAFS.Application.Users
{
    public class UserProfileAppService : ApplicationService, IUserProfileAppService
    {
        private readonly IIdentityUserRepository _userRepository;
        private readonly IdentityUserManager _userManager;
        private readonly IRepository<Experience, Guid> _experienceRepository;
        private readonly IRepository<FavoriteDestination, Guid> _favoriteRepository;

        public UserProfileAppService(
            IIdentityUserRepository userRepository,
            IdentityUserManager userManager,
            IRepository<Experience, Guid> experienceRepository,
            IRepository<FavoriteDestination, Guid> favoriteRepository)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _experienceRepository = experienceRepository;
            _favoriteRepository = favoriteRepository;
        }

        public async Task<PublicUserProfileDto> GetPublicProfileAsync(Guid userId)
        {
            var user = await _userRepository.FindAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("Usuario no encontrado.");
            }

            var experiences = await _experienceRepository.GetListAsync(e => e.UserId == userId);
            var favoritesCount = await _favoriteRepository.CountAsync(f => f.UserId == userId);

            var bio = user.GetProperty<string?>("Bio");
            var avatarUrl = user.GetProperty<string?>("AvatarUrl");
            var preferredLang = user.GetProperty<string?>("PreferredLanguage");

            var recentExpDtos = ObjectMapper.Map<List<Experience>, List<ExperienceDto>>(
                experiences.OrderByDescending(e => e.CreationTime).Take(5).ToList()
            );

            return new PublicUserProfileDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Bio = bio,
                AvatarUrl = avatarUrl,
                PreferredLanguage = preferredLang,
                ExperiencesCount = experiences.Count,
                FavoritesCount = favoritesCount,
                RecentExperiences = recentExpDtos
            };
        }

        [Authorize]
        public async Task<PublicUserProfileDto> GetMyProfileAsync()
        {
            if (!CurrentUser.IsAuthenticated || !CurrentUser.Id.HasValue)
                throw new AbpAuthorizationException("Debe estar autenticado.");

            return await GetPublicProfileAsync(CurrentUser.GetId());
        }

        [Authorize]
        public async Task UpdatePreferencesAsync(UpdateUserPreferencesDto input)
        {
            if (!CurrentUser.IsAuthenticated || !CurrentUser.Id.HasValue)
                throw new AbpAuthorizationException("Debe estar autenticado.");

            var user = await _userManager.GetByIdAsync(CurrentUser.GetId());

            if (input.Bio != null)
                user.SetProperty("Bio", input.Bio);
            if (input.AvatarUrl != null)
                user.SetProperty("AvatarUrl", input.AvatarUrl);
            if (input.PreferredLanguage != null)
                user.SetProperty("PreferredLanguage", input.PreferredLanguage);

            await _userManager.UpdateAsync(user);
        }

        [Authorize]
        public async Task DeleteAccountAsync()
        {
            if (!CurrentUser.IsAuthenticated || !CurrentUser.Id.HasValue)
                throw new AbpAuthorizationException("Debe estar autenticado.");

            var userId = CurrentUser.GetId();
            var user = await _userManager.GetByIdAsync(userId);

            // Eliminar experiencias y favoritos del usuario
            await _experienceRepository.DeleteAsync(e => e.UserId == userId);
            await _favoriteRepository.DeleteAsync(f => f.UserId == userId);

            // Eliminar usuario
            await _userManager.DeleteAsync(user);
        }
    }
}
