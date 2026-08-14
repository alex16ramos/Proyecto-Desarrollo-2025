using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace FAFS.Application.Contracts.Destinations
{
    public interface ITicketmasterService : IApplicationService
    {
        Task<List<DestinationEventDto>> GetEventsByCityAsync(string city, string? countryCode = null);
    }
}
