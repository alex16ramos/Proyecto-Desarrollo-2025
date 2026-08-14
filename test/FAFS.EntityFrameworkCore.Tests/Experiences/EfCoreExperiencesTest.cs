using System;
using System.Threading.Tasks;
using FAFS.EntityFrameworkCore;
using FAFS.Experiences;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace FAFS.Experiences
{
    [Collection(FAFSTestConsts.CollectionDefinitionName)]
    public class EfCoreExperiencesTest : FAFSEntityFrameworkCoreTestBase
    {
        private readonly IRepository<Experience, Guid> _experienceRepository;

        public EfCoreExperiencesTest()
        {
            _experienceRepository = GetRequiredService<IRepository<Experience, Guid>>();
        }

        [Fact]
        public async Task Should_Persist_And_Query_Experiences()
        {
            var id = Guid.NewGuid();
            var destinationId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var exp = new Experience(
                id,
                destinationId,
                userId,
                "Caminata nocturna",
                "Experiencia bajo las estrellas.",
                DateTime.UtcNow,
                5,
                "Llevar linterna."
            );

            await _experienceRepository.InsertAsync(exp, autoSave: true);

            var retrieved = await _experienceRepository.GetAsync(id);
            retrieved.ShouldNotBeNull();
            retrieved.Title.ShouldBe("Caminata nocturna");
            retrieved.Rating.ShouldBe(5);
        }
    }
}
