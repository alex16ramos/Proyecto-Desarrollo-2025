using System;
using System.Threading.Tasks;
using FAFS.Application.Contracts.Destinations;
using FAFS.Application.Contracts.Favorites;
using FAFS.Destinations;
using Shouldly;
using Xunit;

namespace FAFS.Application.Tests.Favorites
{
    public class FavoriteDestinationAppService_Tests : FAFSApplicationTestBase
    {
        private readonly IFavoriteDestinationAppService _favoriteAppService;
        private readonly IDestinationAppService _destinationAppService;

        public FavoriteDestinationAppService_Tests()
        {
            _favoriteAppService = GetRequiredService<IFavoriteDestinationAppService>();
            _destinationAppService = GetRequiredService<IDestinationAppService>();
        }

        [Fact]
        public async Task Should_Toggle_And_Retrieve_Favorites()
        {
            // Arrange
            var destination = await _destinationAppService.CreateAsync(new CreateUpdateDestinationDto
            {
                Name = "Ushuaia",
                Country = "Argentina",
                City = "Ushuaia",
                Latitude = "-54.8019",
                Longitude = "-68.3030"
            });

            var userId = Guid.NewGuid();

            using (Login(userId))
            {
                // Act 1: Toggle favorite ON
                var added = await _favoriteAppService.ToggleFavoriteAsync(destination.Id);
                added.ShouldBeTrue();

                var isFav = await _favoriteAppService.IsFavoriteAsync(destination.Id);
                isFav.ShouldBeTrue();

                var favorites = await _favoriteAppService.GetUserFavoritesAsync();
                favorites.Count.ShouldBeGreaterThanOrEqualTo(1);
                favorites.ShouldContain(f => f.Id == destination.Id);

                // Act 2: Toggle favorite OFF
                var removed = await _favoriteAppService.ToggleFavoriteAsync(destination.Id);
                removed.ShouldBeFalse();

                var isFavAfter = await _favoriteAppService.IsFavoriteAsync(destination.Id);
                isFavAfter.ShouldBeFalse();
            }
        }
    }
}
