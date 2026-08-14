using System;
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
}
