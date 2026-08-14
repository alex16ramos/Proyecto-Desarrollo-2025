using System;
using Volo.Abp.Domain.Entities;

namespace FAFS.Metrics
{
    public class ApiUsageMetric : AggregateRoot<Guid>
    {
        public string Endpoint { get; set; } = string.Empty;
        public string HttpMethod { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public long ExecutionDurationMs { get; set; }
        public Guid? UserId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? IpAddress { get; set; }

        protected ApiUsageMetric() { }

        public ApiUsageMetric(
            Guid id,
            string endpoint,
            string httpMethod,
            int statusCode,
            long executionDurationMs,
            Guid? userId = null,
            string? ipAddress = null
        ) : base(id)
        {
            Endpoint = endpoint;
            HttpMethod = httpMethod;
            StatusCode = statusCode;
            ExecutionDurationMs = executionDurationMs;
            UserId = userId;
            Timestamp = DateTime.UtcNow;
            IpAddress = ipAddress;
        }
    }
}
