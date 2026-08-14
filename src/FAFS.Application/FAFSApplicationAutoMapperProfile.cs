using AutoMapper;
using FAFS.Application.Contracts.Destinations;
using FAFS.Application.Contracts.Experiences;
using FAFS.Application.Contracts.Favorites;
using FAFS.Application.Contracts.Metrics;
using FAFS.Application.Contracts.Notifications;
using FAFS.Destinations;
using FAFS.Experiences;
using FAFS.Favorites;
using FAFS.Metrics;
using FAFS.Notifications;

namespace FAFS
{
    public class FAFSApplicationAutoMapperProfile : Profile
    {
        public FAFSApplicationAutoMapperProfile()
        {
            CreateMap<Destination, DestinationDto>()
                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Coordinates != null ? src.Coordinates.Latitude : string.Empty))
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Coordinates != null ? src.Coordinates.Longitude : string.Empty))
                .ForMember(dest => dest.AverageRating, opt => opt.Ignore())
                .ForMember(dest => dest.RatingsCount, opt => opt.Ignore())
                .ForMember(dest => dest.IsFavorite, opt => opt.Ignore());

            CreateMap<CreateUpdateDestinationDto, Destination>()
                .ForMember(dest => dest.Coordinates, opt => opt.MapFrom(src => new Coordinates(src.Latitude ?? string.Empty, src.Longitude ?? string.Empty)));

            CreateMap<Experience, ExperienceDto>()
                .ForMember(dest => dest.DestinationName, opt => opt.Ignore());

            CreateMap<CreateUpdateExperienceDto, Experience>();

            CreateMap<DestinationRating, DestinationRatingDto>();

            CreateMap<FavoriteDestination, FavoriteDestinationDto>()
                .ForMember(dest => dest.Destination, opt => opt.Ignore());

            CreateMap<ApiUsageMetric, ApiUsageMetricDto>();
            CreateMap<CreateApiUsageMetricDto, ApiUsageMetric>();

            CreateMap<AppNotification, NotificationDto>();
        }
    }
}