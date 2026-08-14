using System;
using System.Threading.Tasks;
using FAFS.Application.Contracts.Destinations;
using Shouldly;
using Volo.Abp.Authorization;
using Xunit;

namespace FAFS.Application.Tests.Destinations
{
    public class DestinationRatingSecurity_Tests : FAFSApplicationTestBase
    {
        private readonly IDestinationRatingAppService _ratingAppService;

        public DestinationRatingSecurity_Tests()
        {
            _ratingAppService = GetRequiredService<IDestinationRatingAppService>();
        }

        [Fact]
        public async Task Should_Throw_AuthorizationException_If_Not_Authenticated()
        {
            // Arrange: explicit unauthenticated principal
            var destinationId = Guid.NewGuid();

            // Act & Assert
            using (CurrentPrincipalAccessor.Change(new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity())))
            {
                await Should.ThrowAsync<AbpAuthorizationException>(async () =>
                {
                    await _ratingAppService.RateDestinationAsync(destinationId, 5, "SinToken");
                });
            }
        }
    }
}
