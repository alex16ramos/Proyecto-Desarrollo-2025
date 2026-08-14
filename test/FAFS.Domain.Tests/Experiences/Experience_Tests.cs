using System;
using FAFS.Experiences;
using Shouldly;
using Xunit;

namespace FAFS.Domain.Tests.Experiences
{
    public class Experience_Tests : FAFSDomainTestBase
    {
        [Fact]
        public void Should_Create_Valid_Experience()
        {
            var id = Guid.NewGuid();
            var destinationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var date = DateTime.UtcNow;

            var experience = new Experience(
                id,
                destinationId,
                userId,
                "Trekking en el bosque",
                "Hermoso sendero con vistas panorámicas.",
                date,
                5,
                "Llevar buen calzado."
            );

            experience.Id.ShouldBe(id);
            experience.DestinationId.ShouldBe(destinationId);
            experience.UserId.ShouldBe(userId);
            experience.Title.ShouldBe("Trekking en el bosque");
            experience.Description.ShouldBe("Hermoso sendero con vistas panorámicas.");
            experience.DateExperienced.ShouldBe(date);
            experience.Rating.ShouldBe(5);
            experience.Comments.ShouldBe("Llevar buen calzado.");
        }
    }
}
