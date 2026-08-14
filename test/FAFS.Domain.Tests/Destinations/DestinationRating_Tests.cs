using System;
using System.Collections.Generic;
using FAFS.Destinations;
using Shouldly;
using Volo.Abp;
using Xunit;

namespace FAFS.Domain.Tests.Destinations
{
    public class DestinationRating_Tests : FAFSDomainTestBase
    {
        private readonly DestinationManager _destinationManager;

        public DestinationRating_Tests()
        {
            _destinationManager = GetRequiredService<DestinationManager>();
        }

        [Fact]
        public void Should_Create_DestinationRating()
        {
            var id = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var destinationId = Guid.NewGuid();

            var rating = new DestinationRating(id, userId, destinationId, 5, "Excelente");

            rating.Id.ShouldBe(id);
            rating.UserId.ShouldBe(userId);
            rating.DestinationId.ShouldBe(destinationId);
            rating.Score.ShouldBe(5);
            rating.Comment.ShouldBe("Excelente");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        public void ValidateRatingScore_Should_Pass_For_Valid_Scores(int score)
        {
            Should.NotThrow(() => _destinationManager.ValidateRatingScore(score));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(6)]
        [InlineData(10)]
        public void ValidateRatingScore_Should_Throw_For_Invalid_Scores(int score)
        {
            Should.Throw<BusinessException>(() => _destinationManager.ValidateRatingScore(score));
        }

        [Fact]
        public void CalculateAverage_Should_Compute_Correctly()
        {
            var scores = new List<int> { 5, 4, 3, 5, 4 };
            var avg = _destinationManager.CalculateAverage(scores);
            avg.ShouldBe(4.2);
        }

        [Fact]
        public void CalculateAverage_Should_Return_Zero_When_Empty()
        {
            var scores = new List<int>();
            var avg = _destinationManager.CalculateAverage(scores);
            avg.ShouldBe(0.0);
        }
    }
}
