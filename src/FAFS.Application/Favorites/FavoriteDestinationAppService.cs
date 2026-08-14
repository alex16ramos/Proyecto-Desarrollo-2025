using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FAFS.Application.Contracts.Destinations;
using FAFS.Application.Contracts.Favorites;
using FAFS.Destinations;
using FAFS.Favorites;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace FAFS.Application.Favorites
{
    [Authorize]
    public class FavoriteDestinationAppService : ApplicationService, IFavoriteDestinationAppService
    {
        private readonly IRepository<FavoriteDestination, Guid> _favoriteRepository;
        private readonly IRepository<Destination, Guid> _destinationRepository;

        public FavoriteDestinationAppService(
            IRepository<FavoriteDestination, Guid> favoriteRepository,
            IRepository<Destination, Guid> destinationRepository)
        {
            _favoriteRepository = favoriteRepository;
            _destinationRepository = destinationRepository;
        }

        public async Task<bool> ToggleFavoriteAsync(Guid destinationId)
        {
            if (!CurrentUser.IsAuthenticated)
            {
                throw new AbpAuthorizationException("Debe estar autenticado para modificar favoritos.");
            }

            var userId = CurrentUser.GetId();
            var existing = (await _favoriteRepository.GetListAsync(f => f.UserId == userId && f.DestinationId == destinationId)).FirstOrDefault();

            if (existing != null)
            {
                await _favoriteRepository.DeleteAsync(existing, autoSave: true);
                return false; // Removed from favorites
            }
            else
            {
                var fav = new FavoriteDestination(GuidGenerator.Create(), userId, destinationId);
                await _favoriteRepository.InsertAsync(fav, autoSave: true);
                return true; // Added to favorites
            }
        }

        public async Task<bool> IsFavoriteAsync(Guid destinationId)
        {
            if (!CurrentUser.IsAuthenticated)
            {
                return false;
            }

            var userId = CurrentUser.GetId();
            return await _favoriteRepository.AnyAsync(f => f.UserId == userId && f.DestinationId == destinationId);
        }

        public async Task<List<DestinationDto>> GetUserFavoritesAsync()
        {
            if (!CurrentUser.IsAuthenticated)
            {
                throw new AbpAuthorizationException("Debe estar autenticado.");
            }

            var userId = CurrentUser.GetId();
            var favorites = await _favoriteRepository.GetListAsync(f => f.UserId == userId);
            var destinationIds = favorites.Select(f => f.DestinationId).ToList();

            if (!destinationIds.Any())
            {
                return new List<DestinationDto>();
            }

            var destinations = await _destinationRepository.GetListAsync(d => destinationIds.Contains(d.Id));
            var dtos = ObjectMapper.Map<List<Destination>, List<DestinationDto>>(destinations);

            foreach (var dto in dtos)
            {
                dto.IsFavorite = true;
            }

            return dtos;
        }
    }
}
