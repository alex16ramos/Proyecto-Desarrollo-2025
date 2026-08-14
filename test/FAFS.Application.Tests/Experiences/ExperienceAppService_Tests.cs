using System;
using System.Threading.Tasks;
using FAFS.Application.Contracts.Destinations;
using FAFS.Application.Contracts.Experiences;
using FAFS.Destinations;
using FAFS.Experiences;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace FAFS.Application.Tests.Experiences
{
    public class ExperienceAppService_Tests : FAFSApplicationTestBase
    {
        private readonly IExperienceAppService _experienceAppService;
        private readonly IDestinationAppService _destinationAppService;
        private readonly IRepository<Experience, Guid> _experienceRepository;

        public ExperienceAppService_Tests()
        {
            _experienceAppService = GetRequiredService<IExperienceAppService>();
            _destinationAppService = GetRequiredService<IDestinationAppService>();
            _experienceRepository = GetRequiredService<IRepository<Experience, Guid>>();
        }

        [Fact]
        public async Task Should_Create_And_Get_Experience()
        {
            // Arrange
            var destination = await _destinationAppService.CreateAsync(new CreateUpdateDestinationDto
            {
                Name = "Mendoza",
                Country = "Argentina",
                City = "Mendoza",
                Latitude = "-32.8895",
                Longitude = "-68.8458"
            });

            var userId = Guid.NewGuid();

            // Act
            ExperienceDto result;
            using (Login(userId))
            {
                result = await _experienceAppService.CreateAsync(new CreateUpdateExperienceDto
                {
                    DestinationId = destination.Id,
                    Title = "Ruta del Vino",
                    Description = "Recorrido por bodegas de Luján de Cuyo.",
                    DateExperienced = DateTime.UtcNow,
                    Rating = 5,
                    Comments = "Excelente gastronomía."
                });
            }

            // Assert
            result.ShouldNotBeNull();
            result.Title.ShouldBe("Ruta del Vino");
            result.UserId.ShouldBe(userId);

            var list = await _experienceAppService.GetListByDestinationAsync(destination.Id);
            list.Count.ShouldBeGreaterThanOrEqualTo(1);
            list.ShouldContain(x => x.Title == "Ruta del Vino");
        }
    }
}
