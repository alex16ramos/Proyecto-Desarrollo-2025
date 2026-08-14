using System;
using FAFS.Destinations;
using Shouldly;
using Xunit;

namespace FAFS.Domain.Tests.Destinations
{
    public class Destination_Tests : FAFSDomainTestBase
    {
        [Fact]
        public void Should_Create_Valid_Destination()
        {
            // Arrange
            var id = Guid.NewGuid();
            var name = "Bariloche";
            var country = "Argentina";
            var city = "San Carlos de Bariloche";
            var photoUrl = "http://example.com/bariloche.jpg";
            var coords = new Coordinates("-41.1343", "-71.3090");
            var description = "Hermoso destino patagónico.";

            // Act
            var destination = new Destination(id, name, country, city, photoUrl, DateTime.UtcNow, coords, description);

            // Assert
            destination.Id.ShouldBe(id);
            destination.Name.ShouldBe(name);
            destination.Country.ShouldBe(country);
            destination.City.ShouldBe(city);
            destination.PhotoUrl.ShouldBe(photoUrl);
            destination.Coordinates.Latitude.ShouldBe("-41.1343");
            destination.Coordinates.Longitude.ShouldBe("-71.3090");
            destination.Description.ShouldBe(description);
        }

        [Fact]
        public void Coordinates_ValueObject_Equality_Should_Work()
        {
            var c1 = new Coordinates("-34.6037", "-58.3816");
            var c2 = new Coordinates("-34.6037", "-58.3816");
            var c3 = new Coordinates("-41.1343", "-71.3090");

            c1.Equals(c2).ShouldBeTrue();
            c1.Equals(c3).ShouldBeFalse();
            c1.Latitude.ShouldBe(c2.Latitude);
            c1.Longitude.ShouldBe(c2.Longitude);
        }
    }
}
