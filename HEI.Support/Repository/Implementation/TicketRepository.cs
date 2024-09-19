using HEI.Support.Data;
using HEI.Support.Data.Entities;
using HEI.Support.Repository.Interface;
using Microsoft.Extensions.Hosting;

namespace HEI.Support.Repository.Implementation
{
    public class TicketRepository : BaseRepository<Ticket>, IBaseRepository<Ticket>, ITicketRepository
    {
        public TicketRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {

        }
    }
}
