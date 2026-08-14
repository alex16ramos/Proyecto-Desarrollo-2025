using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace FAFS.Application.Contracts.Metrics
{
    public class ApiUsageMetricDto : EntityDto<Guid>
    {
        public string Endpoint { get; set; } = string.Empty;
        public string HttpMethod { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public long ExecutionDurationMs { get; set; }
        public Guid? UserId { get; set; }
        public DateTime Timestamp { get; set; }
        public string? IpAddress { get; set; }
    }

    public class CreateApiUsageMetricDto
    {
        public string Endpoint { get; set; } = string.Empty;
        public string HttpMethod { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public long ExecutionDurationMs { get; set; }
        public string? IpAddress { get; set; }
    }

    public class GetApiUsageMetricsInput : PagedAndSortedResultRequestDto
    {
        public string? Endpoint { get; set; }
        public string? HttpMethod { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class ApiUsageStatsDto
    {
        public long TotalRequests { get; set; }
        public long SuccessCount { get; set; }
        public long ErrorCount { get; set; }
        public double SuccessRate { get; set; }
        public double ErrorRate { get; set; }
        public double AverageDurationMs { get; set; }
        public List<TopEndpointDto> TopSlowEndpoints { get; set; } = new();
        public List<TopEndpointDto> TopRequestedEndpoints { get; set; } = new();
    }

    public class TopEndpointDto
    {
        public string Endpoint { get; set; } = string.Empty;
        public string HttpMethod { get; set; } = string.Empty;
        public long RequestCount { get; set; }
        public double AverageDurationMs { get; set; }
        public long MaxDurationMs { get; set; }
    }
}
