using HEI.Support.Domain.Entities;
using HEI.Support.Infrastructure.Persistence.Repository.Interface;

namespace HEI.Support.Infrastructure.Persistence.Repository.Implementation
{
    public class TicketRepository : BaseRepository<Ticket>, IBaseRepository<Ticket>, ITicketRepository
    {
        public TicketRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {

        }
    }
}
