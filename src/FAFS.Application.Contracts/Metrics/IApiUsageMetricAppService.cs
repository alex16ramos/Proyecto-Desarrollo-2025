using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace FAFS.Application.Contracts.Metrics
{
    public interface IApiUsageMetricAppService : IApplicationService
    {
        Task<ApiUsageMetricDto> RecordMetricAsync(CreateApiUsageMetricDto input);
        Task<PagedResultDto<ApiUsageMetricDto>> GetListAsync(GetApiUsageMetricsInput input);
        Task<ApiUsageStatsDto> GetStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    }
}
