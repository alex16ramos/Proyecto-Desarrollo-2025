using System;
using System.Threading.Tasks;
using FAFS.Destinations;
using FAFS.EntityFrameworkCore;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace FAFS.Destinations
{
    [Collection(FAFSTestConsts.CollectionDefinitionName)]
    public class EfCoreDestinationsTest : FAFSEntityFrameworkCoreTestBase
    {
        private readonly IRepository<Destination, Guid> _destinationRepository;

        public EfCoreDestinationsTest()
        {
            _destinationRepository = GetRequiredService<IRepository<Destination, Guid>>();
        }

        [Fact]
        public async Task Should_Persist_Destination_With_Coordinates_ValueObject()
        {
            var id = Guid.NewGuid();
            var destination = new Destination(
                id,
                "Salta La Linda",
                "Argentina",
                "Salta",
                "http://example.com/salta.jpg",
                DateTime.UtcNow,
                new Coordinates("-24.7821", "-65.4232"),
                "Ciudad colonial en el noroeste argentino."
            );

            await _destinationRepository.InsertAsync(destination, autoSave: true);

            var retrieved = await _destinationRepository.GetAsync(id);
            retrieved.ShouldNotBeNull();
            retrieved.Name.ShouldBe("Salta La Linda");
            retrieved.Coordinates.Latitude.ShouldBe("-24.7821");
            retrieved.Coordinates.Longitude.ShouldBe("-65.4232");
        }
    }
}
