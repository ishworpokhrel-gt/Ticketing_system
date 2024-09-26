using HEI.Support.Common.Models;
using HEI.Support.Common.Models.Enum;
using HEI.Support.Domain.Entities;
using HEI.Support.Infrastructure.Persistence.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace HEI.Support.Infrastructure.Persistence.Repository.Implementation
{
    public class TicketRepository : BaseRepository<Ticket>, IBaseRepository<Ticket>, ITicketRepository
    {
        private readonly ApplicationDbContext _context;
        public TicketRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _context = applicationDbContext;
        }
        public async Task<List<TicketViewModel>> GetAllTicketsAsync()
        {
            var data = await _context.Tickets
                .Include(t => t.ActivityLogs)
                .ThenInclude(a => a.User)
                .OrderByDescending(t => t.CreatedDate)
                .Select(ticket => new TicketViewModel
                {
                    Id = ticket.Id,
                    IssueType = ticket.IssueTypeId,
                    Description = ticket.Description,
                    Priority = ticket.Priority,
                    Status = ticket.Status,
                    AsignTo = ticket.ActivityLogs
                        .Where(al => al.Status == (int)TicketStatus.InProgress)
                        .OrderByDescending(al => al.CreatedDate)
                        .Select(al => al.User.UserName)
                        .FirstOrDefault() ?? "",
                    CreatedBy = ticket.ActivityLogs
                        .Where(al => al.Status == (int)TicketStatus.Open)
                        .OrderByDescending(al => al.CreatedDate)
                        .Select(al => al.User.UserName)
                        .FirstOrDefault() ?? ""
                })
                .ToListAsync();

            return data;
        }

        public async Task<string> GetTicketAssignee(Guid ticketId, int status)
        {
            var assignee = await _context.ActivityLogs
                         .Include(a => a.User)
                         .Where(al => al.Status == status && al.TicketId==ticketId)
                         .Select(al => al.User.FirstName + " " + al.User.LastName)
                         .FirstOrDefaultAsync();

            return assignee??"";
        }


    }
}
