using System;
using System.Threading.Tasks;
using FAFS.Application.Contracts.Metrics;
using Shouldly;
using Xunit;

namespace FAFS.Application.Tests.Metrics
{
    public class ApiUsageMetricAppService_Tests : FAFSApplicationTestBase
    {
        private readonly IApiUsageMetricAppService _metricAppService;

        public ApiUsageMetricAppService_Tests()
        {
            _metricAppService = GetRequiredService<IApiUsageMetricAppService>();
        }

        [Fact]
        public async Task Should_Record_And_Query_Api_Usage_Metrics()
        {
            // Act
            var recorded = await _metricAppService.RecordMetricAsync(new CreateApiUsageMetricDto
            {
                Endpoint = "/api/app/destination",
                HttpMethod = "GET",
                StatusCode = 200,
                ExecutionDurationMs = 120,
                IpAddress = "192.168.1.100"
            });

            // Assert
            recorded.ShouldNotBeNull();
            recorded.Endpoint.ShouldBe("/api/app/destination");
            recorded.StatusCode.ShouldBe(200);

            var list = await _metricAppService.GetListAsync(new GetApiUsageMetricsInput
            {
                Endpoint = "destination"
            });

            list.TotalCount.ShouldBeGreaterThanOrEqualTo(1);
            list.Items.ShouldContain(m => m.Id == recorded.Id);
        }
    }
}
