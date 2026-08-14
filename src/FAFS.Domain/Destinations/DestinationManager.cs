using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace FAFS.Destinations
{
    public class DestinationManager : DomainService
    {
        private readonly IRepository<DestinationRating, Guid> _ratingRepository;
        private readonly IRepository<Destination, Guid> _destinationRepository;

        public DestinationManager(
            IRepository<DestinationRating, Guid> ratingRepository,
            IRepository<Destination, Guid> destinationRepository)
        {
            _ratingRepository = ratingRepository;
            _destinationRepository = destinationRepository;
        }

        public void ValidateRatingScore(int score)
        {
            if (score < 1 || score > 5)
            {
                throw new BusinessException("FAFS:InvalidRatingScore")
                    .WithData("Score", score)
                    .WithData("Message", "La calificación debe estar entre 1 y 5 estrellas.");
            }
        }

        public double CalculateAverage(IEnumerable<int> scores)
        {
            if (scores == null || !scores.Any())
            {
                return 0.0;
            }

            return Math.Round(scores.Average(), 1);
        }

        public async Task<(double averageRating, int totalRatings)> GetDestinationRatingSummaryAsync(Guid destinationId)
        {
            var ratings = await _ratingRepository.GetListAsync(r => r.DestinationId == destinationId);
            if (!ratings.Any())
            {
                return (0.0, 0);
            }

            var avg = Math.Round(ratings.Average(r => r.Score), 1);
            return (avg, ratings.Count);
        }
    }
}
