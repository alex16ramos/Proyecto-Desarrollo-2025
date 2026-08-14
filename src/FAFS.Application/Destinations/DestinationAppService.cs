using FAFS.Application.Contracts.Destinations;
using FAFS.Favorites;
using FAFS.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace FAFS.Destinations
{
    public class DestinationAppService :
     CrudAppService<
         Destination,
         DestinationDto,
         Guid,
         PagedAndSortedResultRequestDto,
         CreateUpdateDestinationDto>,
     IDestinationAppService
    {
        private readonly ICitySearchService _citySearchService;
        private readonly IRepository<DestinationRating, Guid>? _ratingRepository;
        private readonly IRepository<FavoriteDestination, Guid>? _favoriteRepository;

        public DestinationAppService(
            IRepository<Destination, Guid> repository,
            ICitySearchService citySearchService,
            IAbpLazyServiceProvider? lazyServiceProvider = null,
            IRepository<DestinationRating, Guid>? ratingRepository = null,
            IRepository<FavoriteDestination, Guid>? favoriteRepository = null)
            : base(repository)
        {
            _citySearchService = citySearchService;
            if (lazyServiceProvider != null)
            {
                LazyServiceProvider = lazyServiceProvider;
            }
            _ratingRepository = ratingRepository;
            _favoriteRepository = favoriteRepository;

            GetPolicyName = null;
            GetListPolicyName = null;
            CreatePolicyName = null;
            UpdatePolicyName = null;
            DeletePolicyName = null;
        }

        public async Task<CitySearchResultDto> SearchCitiesAsync(CitySearchRequestDto request)
        {
            return await _citySearchService.SearchCitiesAsync(request);
        }

        public override async Task<DestinationDto> GetAsync(Guid id)
        {
            var dto = await base.GetAsync(id);
            await EnrichDestinationDtoAsync(dto);
            return dto;
        }

        public override async Task<PagedResultDto<DestinationDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            var result = await base.GetListAsync(input);
            foreach (var item in result.Items)
            {
                await EnrichDestinationDtoAsync(item);
            }
            return result;
        }

        public async Task<List<DestinationDto>> GetPopularDestinationsAsync(int count = 6)
        {
            var destinations = await Repository.GetListAsync();
            var dtos = ObjectMapper.Map<List<Destination>, List<DestinationDto>>(destinations);
            foreach (var dto in dtos)
            {
                await EnrichDestinationDtoAsync(dto);
            }

            return dtos
                .OrderByDescending(d => d.AverageRating)
                .ThenByDescending(d => d.RatingsCount)
                .Take(count)
                .ToList();
        }

        private async Task EnrichDestinationDtoAsync(DestinationDto dto)
        {
            if (_ratingRepository != null)
            {
                var ratings = await _ratingRepository.GetListAsync(r => r.DestinationId == dto.Id);
                if (ratings.Any())
                {
                    dto.AverageRating = Math.Round(ratings.Average(r => r.Score), 1);
                    dto.RatingsCount = ratings.Count;
                }
            }

            if (_favoriteRepository != null && CurrentUser.IsAuthenticated && CurrentUser.Id.HasValue)
            {
                var userId = CurrentUser.GetId();
                dto.IsFavorite = await _favoriteRepository.AnyAsync(f => f.DestinationId == dto.Id && f.UserId == userId);
            }
        }
    }
}
