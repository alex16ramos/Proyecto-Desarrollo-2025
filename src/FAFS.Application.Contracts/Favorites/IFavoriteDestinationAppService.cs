using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using FAFS.Application.Contracts.Destinations;

namespace FAFS.Application.Contracts.Favorites
{
    public interface IFavoriteDestinationAppService : IApplicationService
    {
        Task<bool> ToggleFavoriteAsync(Guid destinationId);
        Task<bool> IsFavoriteAsync(Guid destinationId);
        Task<List<DestinationDto>> GetUserFavoritesAsync();
    }
}
