using System;
using System.Threading.Tasks;
using FAFS.EntityFrameworkCore;
using FAFS.Favorites;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace FAFS.Favorites
{
    [Collection(FAFSTestConsts.CollectionDefinitionName)]
    public class EfCoreFavoritesTest : FAFSEntityFrameworkCoreTestBase
    {
        private readonly IRepository<FavoriteDestination, Guid> _favoriteRepository;

        public EfCoreFavoritesTest()
        {
            _favoriteRepository = GetRequiredService<IRepository<FavoriteDestination, Guid>>();
        }

        [Fact]
        public async Task Should_Persist_Favorite_Destination()
        {
            var id = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var destinationId = Guid.NewGuid();

            var fav = new FavoriteDestination(id, userId, destinationId);
            await _favoriteRepository.InsertAsync(fav, autoSave: true);

            var retrieved = await _favoriteRepository.GetAsync(id);
            retrieved.ShouldNotBeNull();
            retrieved.UserId.ShouldBe(userId);
            retrieved.DestinationId.ShouldBe(destinationId);
        }
    }
}
