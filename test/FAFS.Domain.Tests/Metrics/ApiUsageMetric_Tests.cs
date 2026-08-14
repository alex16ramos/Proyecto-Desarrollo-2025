using System;
using FAFS.Metrics;
using Shouldly;
using Xunit;

namespace FAFS.Domain.Tests.Metrics
{
    public class ApiUsageMetric_Tests : FAFSDomainTestBase
    {
        [Fact]
        public void Should_Create_ApiUsageMetric()
        {
            var id = Guid.NewGuid();
            var endpoint = "/api/app/destinations";
            var httpMethod = "GET";
            var statusCode = 200;
            var durationMs = 45L;
            var userId = Guid.NewGuid();
            var ip = "127.0.0.1";

            var metric = new ApiUsageMetric(id, endpoint, httpMethod, statusCode, durationMs, userId, ip);

            metric.Id.ShouldBe(id);
            metric.Endpoint.ShouldBe(endpoint);
            metric.HttpMethod.ShouldBe(httpMethod);
            metric.StatusCode.ShouldBe(statusCode);
            metric.ExecutionDurationMs.ShouldBe(durationMs);
            metric.UserId.ShouldBe(userId);
            metric.IpAddress.ShouldBe(ip);
            metric.Timestamp.ShouldNotBe(default);
        }
    }
}
