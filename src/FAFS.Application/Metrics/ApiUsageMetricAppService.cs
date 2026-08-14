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
    }
}
