using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using FAFS.Application.Contracts.Metrics;
using FAFS.Metrics;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace FAFS.Application.Metrics
{
    public class ApiUsageMetricAppService : ApplicationService, IApiUsageMetricAppService
    {
        private readonly IRepository<ApiUsageMetric, Guid> _metricRepository;

        public ApiUsageMetricAppService(IRepository<ApiUsageMetric, Guid> metricRepository)
        {
            _metricRepository = metricRepository;
        }

        public async Task<ApiUsageMetricDto> RecordMetricAsync(CreateApiUsageMetricDto input)
        {
            var metric = new ApiUsageMetric(
                GuidGenerator.Create(),
                input.Endpoint,
                input.HttpMethod,
                input.StatusCode,
                input.ExecutionDurationMs,
                CurrentUser.Id,
                input.IpAddress
            );

            await _metricRepository.InsertAsync(metric, autoSave: true);
            return ObjectMapper.Map<ApiUsageMetric, ApiUsageMetricDto>(metric);
        }

        public async Task<PagedResultDto<ApiUsageMetricDto>> GetListAsync(GetApiUsageMetricsInput input)
        {
            var queryable = await _metricRepository.GetQueryableAsync();

            if (!string.IsNullOrWhiteSpace(input.Endpoint))
            {
                queryable = queryable.Where(x => x.Endpoint.Contains(input.Endpoint));
            }

            if (!string.IsNullOrWhiteSpace(input.HttpMethod))
            {
                queryable = queryable.Where(x => x.HttpMethod == input.HttpMethod);
            }

            if (input.StartDate.HasValue)
            {
                queryable = queryable.Where(x => x.Timestamp >= input.StartDate.Value);
            }

            if (input.EndDate.HasValue)
            {
                queryable = queryable.Where(x => x.Timestamp <= input.EndDate.Value);
            }

            var totalCount = await AsyncExecuter.CountAsync(queryable);

            var sorting = !string.IsNullOrWhiteSpace(input.Sorting) ? input.Sorting : "Timestamp desc";
            var query = queryable
                .OrderBy(sorting)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount);

            var items = await AsyncExecuter.ToListAsync(query);

            var dtos = ObjectMapper.Map<List<ApiUsageMetric>, List<ApiUsageMetricDto>>(items);
            return new PagedResultDto<ApiUsageMetricDto>(totalCount, dtos);
        }

        public async Task<ApiUsageStatsDto> GetStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var queryable = await _metricRepository.GetQueryableAsync();

            if (startDate.HasValue)
            {
                queryable = queryable.Where(x => x.Timestamp >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                queryable = queryable.Where(x => x.Timestamp <= endDate.Value);
            }

            var list = await AsyncExecuter.ToListAsync(queryable);
            var totalCount = list.Count;

            if (totalCount == 0)
            {
                return new ApiUsageStatsDto();
            }

            var successCount = list.Count(x => x.StatusCode >= 200 && x.StatusCode < 400);
            var errorCount = list.Count(x => x.StatusCode >= 400);
            var avgDuration = list.Average(x => x.ExecutionDurationMs);

            var grouped = list
                .GroupBy(x => new { x.Endpoint, x.HttpMethod })
                .Select(g => new TopEndpointDto
                {
                    Endpoint = g.Key.Endpoint,
                    HttpMethod = g.Key.HttpMethod,
                    RequestCount = g.Count(),
                    AverageDurationMs = Math.Round(g.Average(x => x.ExecutionDurationMs), 2),
                    MaxDurationMs = g.Max(x => x.ExecutionDurationMs)
                })
                .ToList();

            var topSlow = grouped
                .OrderByDescending(x => x.AverageDurationMs)
                .Take(10)
                .ToList();

            var topRequested = grouped
                .OrderByDescending(x => x.RequestCount)
                .Take(10)
                .ToList();

            return new ApiUsageStatsDto
            {
                TotalRequests = totalCount,
                SuccessCount = successCount,
                ErrorCount = errorCount,
                SuccessRate = Math.Round((double)successCount / totalCount * 100, 2),
                ErrorRate = Math.Round((double)errorCount / totalCount * 100, 2),
                AverageDurationMs = Math.Round(avgDuration, 2),
                TopSlowEndpoints = topSlow,
                TopRequestedEndpoints = topRequested
            };
        }
    }
}
