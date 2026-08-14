using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace FAFS.Application.Contracts.Destinations
{
    public interface IDestinationRatingAppService : IApplicationService
    {
        Task RateDestinationAsync(Guid destinationId, int score, string? comment);
        Task UpdateMyRatingAsync(Guid ratingId, int score, string? comment);
        Task DeleteMyRatingAsync(Guid ratingId);
        Task<DestinationRatingDto?> GetMyRatingForDestinationAsync(Guid destinationId);
        Task<DestinationRatingSummaryDto> GetDestinationRatingSummaryAsync(Guid destinationId);
        Task<List<DestinationRatingDto>> GetRatingsForDestinationAsync(Guid destinationId);
    }
}
