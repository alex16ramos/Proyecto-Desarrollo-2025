using FAFS.Application.Contracts.Destinations;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace FAFS.Destinations
{
    public class DestinationRatingAppService : ApplicationService, IDestinationRatingAppService
    {
        private readonly IRepository<DestinationRating, Guid> _ratingRepository;
        private readonly IIdentityUserRepository? _userRepository;

        public DestinationRatingAppService(
            IRepository<DestinationRating, Guid> ratingRepository,
            IAbpLazyServiceProvider lazyServiceProvider,
            IIdentityUserRepository? userRepository = null)
        {
            _ratingRepository = ratingRepository;
            LazyServiceProvider = lazyServiceProvider;
            _userRepository = userRepository;
        }

        [Authorize]
        public async Task RateDestinationAsync(Guid destinationId, int score, string? comment)
        {
            if (!CurrentUser.IsAuthenticated || !CurrentUser.Id.HasValue)
                throw new AbpAuthorizationException("Debe estar autenticado para calificar un destino.");

            if (score < 1 || score > 5)
                throw new ArgumentException("La puntuación debe estar entre 1 y 5.");

            var userId = CurrentUser.GetId();
            var existing = (await _ratingRepository.GetListAsync(r => r.DestinationId == destinationId && r.UserId == userId)).FirstOrDefault();

            if (existing != null)
            {
                existing.Score = score;
                existing.Comment = comment;
                existing.CreationTime = DateTime.UtcNow;
                await _ratingRepository.UpdateAsync(existing, autoSave: true);
            }
            else
            {
                var rating = new DestinationRating(
                    GuidGenerator.Create(),
                    userId,
                    destinationId,
                    score,
                    comment
                );

                await _ratingRepository.InsertAsync(rating, autoSave: true);
            }
        }

        [Authorize]
        public async Task UpdateMyRatingAsync(Guid ratingId, int score, string? comment)
        {
            if (!CurrentUser.IsAuthenticated || !CurrentUser.Id.HasValue)
                throw new AbpAuthorizationException("Debe estar autenticado para actualizar su calificación.");

            if (score < 1 || score > 5)
                throw new ArgumentException("La puntuación debe estar entre 1 y 5.");

            var rating = await _ratingRepository.GetAsync(ratingId);
            if (rating.UserId != CurrentUser.GetId())
                throw new AbpAuthorizationException("No tiene permisos para modificar una calificación ajena.");

            rating.Score = score;
            rating.Comment = comment;
            rating.CreationTime = DateTime.UtcNow;

            await _ratingRepository.UpdateAsync(rating, autoSave: true);
        }

        [Authorize]
        public async Task DeleteMyRatingAsync(Guid ratingId)
        {
            if (!CurrentUser.IsAuthenticated || !CurrentUser.Id.HasValue)
                throw new AbpAuthorizationException("Debe estar autenticado para eliminar su calificación.");

            var rating = await _ratingRepository.GetAsync(ratingId);
            if (rating.UserId != CurrentUser.GetId())
                throw new AbpAuthorizationException("No tiene permisos para eliminar una calificación ajena.");

            await _ratingRepository.DeleteAsync(rating, autoSave: true);
        }

        public async Task<DestinationRatingDto?> GetMyRatingForDestinationAsync(Guid destinationId)
        {
            if (!CurrentUser.IsAuthenticated || !CurrentUser.Id.HasValue)
                return null;

            var userId = CurrentUser.GetId();
            var rating = (await _ratingRepository.GetListAsync(r => r.DestinationId == destinationId && r.UserId == userId)).FirstOrDefault();

            if (rating == null)
                return null;

            var dto = ObjectMapper.Map<DestinationRating, DestinationRatingDto>(rating);
            await EnrichRatingDtoAsync(dto);
            return dto;
        }

        public async Task<DestinationRatingSummaryDto> GetDestinationRatingSummaryAsync(Guid destinationId)
        {
            var ratings = await _ratingRepository.GetListAsync(r => r.DestinationId == destinationId);
            if (!ratings.Any())
            {
                return new DestinationRatingSummaryDto
                {
                    DestinationId = destinationId,
                    AverageRating = 0.0,
                    TotalRatings = 0
                };
            }

            return new DestinationRatingSummaryDto
            {
                DestinationId = destinationId,
                AverageRating = Math.Round(ratings.Average(r => r.Score), 1),
                TotalRatings = ratings.Count
            };
        }

        public async Task<List<DestinationRatingDto>> GetRatingsForDestinationAsync(Guid destinationId)
        {
            var ratings = await _ratingRepository.GetListAsync(r => r.DestinationId == destinationId);
            var dtos = ObjectMapper.Map<List<DestinationRating>, List<DestinationRatingDto>>(
                ratings.OrderByDescending(r => r.CreationTime).ToList()
            );

            foreach (var dto in dtos)
            {
                await EnrichRatingDtoAsync(dto);
            }

            return dtos;
        }

        private async Task EnrichRatingDtoAsync(DestinationRatingDto dto)
        {
            if (_userRepository != null && dto.UserId != Guid.Empty)
            {
                var user = await _userRepository.FindAsync(dto.UserId);
                if (user != null)
                {
                    dto.UserName = !string.IsNullOrWhiteSpace(user.Name) ? $"{user.Name} {user.Surname}".Trim() : user.UserName;
                    dto.UserAvatarUrl = user.GetProperty<string?>("AvatarUrl");
                }
            }
        }
    }
}

