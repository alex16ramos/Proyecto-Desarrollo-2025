using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace FAFS.Application.Contracts.Experiences
{
    public interface IExperienceAppService :
        ICrudAppService<
            ExperienceDto,
            Guid,
            GetExperiencesInput,
            CreateUpdateExperienceDto>
    {
        Task<List<ExperienceDto>> GetListByDestinationAsync(Guid destinationId);
        Task<List<ExperienceDto>> GetUserExperiencesAsync();
    }
}
