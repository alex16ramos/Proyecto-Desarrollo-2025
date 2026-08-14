using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FAFS.Application.Contracts.Experiences;
using FAFS.Destinations;
using FAFS.Experiences;
using FAFS.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace FAFS.Application.Experiences
{
    public class ExperienceAppService :
        CrudAppService<
            Experience,
            ExperienceDto,
            Guid,
            GetExperiencesInput,
            CreateUpdateExperienceDto>,
        IExperienceAppService
    {
        private readonly IRepository<Destination, Guid> _destinationRepository;

        public ExperienceAppService(
            IRepository<Experience, Guid> repository,
            IRepository<Destination, Guid> destinationRepository)
            : base(repository)
        {
            _destinationRepository = destinationRepository;

            GetPolicyName = null;
            GetListPolicyName = null;
            CreatePolicyName = FAFSPermissions.Experiences.Create;
            UpdatePolicyName = FAFSPermissions.Experiences.Edit;
            DeletePolicyName = FAFSPermissions.Experiences.Delete;
        }

        public override async Task<ExperienceDto> CreateAsync(CreateUpdateExperienceDto input)
        {
            if (CurrentUser.IsAuthenticated && CurrentUser.Id.HasValue)
            {
                var experience = new Experience(
                    GuidGenerator.Create(),
                    input.DestinationId,
                    CurrentUser.GetId(),
                    input.Title,
                    input.Description,
                    input.DateExperienced,
                    input.Rating,
                    input.Comments
                );

                await Repository.InsertAsync(experience, autoSave: true);
                var dto = await MapToGetOutputDtoAsync(experience);
                await EnrichExperienceDtoAsync(dto);
                return dto;
            }

            return await base.CreateAsync(input);
        }

        public override async Task<ExperienceDto> UpdateAsync(Guid id, CreateUpdateExperienceDto input)
        {
            var experience = await Repository.GetAsync(id);
            if (CurrentUser.IsAuthenticated && CurrentUser.Id.HasValue && experience.UserId != CurrentUser.GetId())
            {
                throw new AbpAuthorizationException("No tiene permisos para editar una experiencia ajena.");
            }

            experience.Title = input.Title;
            experience.Description = input.Description;
            experience.DateExperienced = input.DateExperienced;
            experience.Rating = input.Rating;
            experience.Comments = input.Comments;

            await Repository.UpdateAsync(experience, autoSave: true);
            var dto = await MapToGetOutputDtoAsync(experience);
            await EnrichExperienceDtoAsync(dto);
            return dto;
        }

        public override async Task DeleteAsync(Guid id)
        {
            var experience = await Repository.GetAsync(id);
            if (CurrentUser.IsAuthenticated && CurrentUser.Id.HasValue && experience.UserId != CurrentUser.GetId())
            {
                throw new AbpAuthorizationException("No tiene permisos para eliminar una experiencia ajena.");
            }

            await Repository.DeleteAsync(experience, autoSave: true);
        }

        protected override async Task<IQueryable<Experience>> CreateFilteredQueryAsync(GetExperiencesInput input)
        {
            var query = await base.CreateFilteredQueryAsync(input);

            if (input.DestinationId.HasValue)
            {
                query = query.Where(e => e.DestinationId == input.DestinationId.Value);
            }

            if (input.UserId.HasValue)
            {
                query = query.Where(e => e.UserId == input.UserId.Value);
            }

            if (input.MinRating.HasValue)
            {
                query = query.Where(e => e.Rating >= input.MinRating.Value);
            }

            if (input.MaxRating.HasValue)
            {
                query = query.Where(e => e.Rating <= input.MaxRating.Value);
            }

            if (!string.IsNullOrWhiteSpace(input.Filter))
            {
                var filter = input.Filter.Trim().ToLower();
                query = query.Where(e => e.Title.ToLower().Contains(filter) ||
                                         e.Description.ToLower().Contains(filter) ||
                                         (e.Comments != null && e.Comments.ToLower().Contains(filter)));
            }

            return query;
        }

        public async Task<List<ExperienceDto>> GetListByDestinationAsync(Guid destinationId)
        {
            var list = await Repository.GetListAsync(e => e.DestinationId == destinationId);
            var dtos = ObjectMapper.Map<List<Experience>, List<ExperienceDto>>(list);
            foreach (var dto in dtos)
            {
                await EnrichExperienceDtoAsync(dto);
            }
            return dtos;
        }

        [Authorize]
        public async Task<List<ExperienceDto>> GetUserExperiencesAsync()
        {
            if (!CurrentUser.IsAuthenticated)
            {
                throw new AbpAuthorizationException("Debe estar autenticado.");
            }

            var userId = CurrentUser.GetId();
            var list = await Repository.GetListAsync(e => e.UserId == userId);
            var dtos = ObjectMapper.Map<List<Experience>, List<ExperienceDto>>(list);
            foreach (var dto in dtos)
            {
                await EnrichExperienceDtoAsync(dto);
            }
            return dtos;
        }

        private async Task EnrichExperienceDtoAsync(ExperienceDto dto)
        {
            if (dto.DestinationId != Guid.Empty)
            {
                var destination = await _destinationRepository.FindAsync(dto.DestinationId);
                if (destination != null)
                {
                    dto.DestinationName = destination.Name;
                }
            }
        }
    }
}
