using FAFS.Application.Contracts.Destinations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;


namespace FAFS.Destinations
{
    public interface IDestinationAppService :
        ICrudAppService<
            DestinationDto,              // DTO de salida (para mostrar)
            Guid,                         // Tipo de la PK de la entidad
            PagedAndSortedResultRequestDto, // Para paginación y ordenamiento
            CreateUpdateDestinationDto   // DTO para crear/actualizar
        >
    {
        // Custom method for searching cities (external service)
        Task<CitySearchResultDto> SearchCitiesAsync(CitySearchRequestDto input);
        Task<List<DestinationDto>> GetPopularDestinationsAsync(int count = 6);
        Task<List<DestinationEventDto>> GetEventsAsync(Guid destinationId);
        Task<List<DestinationEventDto>> GetEventsByCityAsync(string cityName, string? countryCode = null);
    }
}