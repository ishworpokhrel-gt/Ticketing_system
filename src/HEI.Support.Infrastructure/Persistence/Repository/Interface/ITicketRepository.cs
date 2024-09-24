using HEI.Support.Common.Models;
using HEI.Support.Domain.Entities;

namespace HEI.Support.Infrastructure.Persistence.Repository.Interface
{
    public interface ITicketRepository : IBaseRepository<Ticket>
    {
        Task<List<TicketViewModel>> GetAllTicketsAsync();
    }
}
