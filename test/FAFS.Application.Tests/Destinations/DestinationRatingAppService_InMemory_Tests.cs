using FAFS.Application.Contracts.Destinations;
using FAFS.Destinations;
using Shouldly;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using Xunit;

namespace FAFS.Application.Tests.Destinations
{
    public class DestinationRatingAppService_InMemory_Tests : FAFSApplicationTestBase
    {
        private readonly IDestinationRatingAppService _appService;

        public DestinationRatingAppService_InMemory_Tests()
        {
            _appService = GetRequiredService<IDestinationRatingAppService>();
        }

        [Fact]
        public async Task Should_Filter_By_Current_User()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var destinationId = Guid.NewGuid();

            // Act
            using (Login(userId))
            {
                await _appService.RateDestinationAsync(destinationId, 5, "Increíble");
            }

            // Assert
            var repo = GetRequiredService<IRepository<DestinationRating, Guid>>();
            var ratings = await repo.GetListAsync(r => r.DestinationId == destinationId);

            ratings.ShouldAllBe(r => r.UserId == userId);
            ratings.Count.ShouldBe(1);
            ratings[0].Score.ShouldBe(5);
        }
    }
}
