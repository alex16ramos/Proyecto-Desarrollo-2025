using System;
using System.Collections.Generic;
using FAFS.Application.Contracts.Experiences;
using Volo.Abp.Application.Dtos;

namespace FAFS.Application.Contracts.Users
{
    public class PublicUserProfileDto : EntityDto<Guid>
    {
        public string UserName { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public string? PreferredLanguage { get; set; }
        public int ExperiencesCount { get; set; }
        public int FavoritesCount { get; set; }
        public List<ExperienceDto> RecentExperiences { get; set; } = new();
    }

    public class UpdateUserPreferencesDto
    {
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public string? PreferredLanguage { get; set; }
    }
}
