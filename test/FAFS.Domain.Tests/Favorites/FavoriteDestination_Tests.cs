using System;
using FAFS.Favorites;
using Shouldly;
using Xunit;

namespace FAFS.Domain.Tests.Favorites
{
    public class FavoriteDestination_Tests : FAFSDomainTestBase
    {
        [Fact]
        public void Should_Create_FavoriteDestination()
        {
            var id = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var destinationId = Guid.NewGuid();

            var fav = new FavoriteDestination(id, userId, destinationId);

            fav.Id.ShouldBe(id);
            fav.UserId.ShouldBe(userId);
            fav.DestinationId.ShouldBe(destinationId);
            fav.CreationTime.ShouldNotBe(default);
        }
    }
}
