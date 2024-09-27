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

        public async Task<List<TicketViewModel>> GetAllTicketsAsync(int? statusId = null, int? issueTypeId = null)
        {
            var query = _context.Tickets
                .Include(t => t.ActivityLogs)
                .ThenInclude(a => a.User)
                .AsQueryable(); 
            if (statusId.HasValue)
            {
                query = query.Where(t => t.Status == statusId);
            }
            if (issueTypeId.HasValue)
            {
                query = query.Where(t => t.IssueTypeId == issueTypeId);
            }

            var data = await query
                .OrderByDescending(t => t.CreatedDate)
                .Select(ticket => new TicketViewModel
                {
                    Id = ticket.Id,
                    FullName = ticket.FullName,
                    Phone = ticket.Phone,
                    IssueType = ticket.IssueTypeId,
                    Description = ticket.Description,
                    Priority = ticket.Priority,
                    Status = ticket.Status,
                    AsignTo = ticket.ActivityLogs
                        .Where(al => al.Status == (int)TicketStatus.InProgress)
                        .OrderByDescending(al => al.CreatedDate)
                        .Select(al => al.User.FirstName + " " + al.User.LastName)
                        .FirstOrDefault() ?? "",
                    CreatedBy = ticket.ActivityLogs
                        .Where(al => al.Status == (int)TicketStatus.Open)
                        .OrderByDescending(al => al.CreatedDate)
                        .Select(al => al.User.FirstName + " " + al.User.LastName)
                        .FirstOrDefault() ?? ""
                })
                .ToListAsync();

            return data;
        }

        //public async Task<List<TicketViewModel>> GetAllTicketsAsync(int status)
        //{
        //    var data = await _context.Tickets
        //        .Include(t => t.ActivityLogs)
        //        .ThenInclude(a => a.User)
        //        .OrderByDescending(t => t.CreatedDate)
        //        .Select(ticket => new TicketViewModel
        //        {
        //            Id = ticket.Id,
        //            FullName = ticket.FullName,
        //            Phone = ticket.Phone,
        //            IssueType = ticket.IssueTypeId,
        //            Description = ticket.Description,
        //            Priority = ticket.Priority,
        //            Status = ticket.Status,
        //            AsignTo = ticket.ActivityLogs
        //                .Where(al => al.Status == (int)TicketStatus.InProgress)
        //                .OrderByDescending(al => al.CreatedDate)
        //                .Select(al => al.User.FirstName + " " + al.User.LastName)
        //                .FirstOrDefault() ?? "",
        //            CreatedBy = ticket.ActivityLogs
        //                .Where(al => al.Status == (int)TicketStatus.Open)
        //                .OrderByDescending(al => al.CreatedDate)
        //                .Select(al => al.User.FirstName + " " + al.User.LastName)
        //                .FirstOrDefault() ?? ""
        //        })
        //        .ToListAsync();

        //    return data;
        //}

        public async Task<string> GetTicketAssignee(Guid ticketId, int status)
        {
            var assignee = await _context.ActivityLogs
                         .Include(a => a.User)
                         .Where(al => al.Status == status && al.TicketId == ticketId)
                         .Select(al => al.User.FirstName + " " + al.User.LastName)
                         .FirstOrDefaultAsync();

            return assignee ?? "";
        }


        public async Task<TicketViewModel> GetTicketByIdAsync(Guid id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Attachments)
                .Include(t => t.ActivityLogs)
                .ThenInclude(al => al.User)
                .Where(t => t.Id == id)
                .OrderByDescending(t => t.CreatedDate)
                .Select(t => new TicketViewModel
                {
                    Id = t.Id,
                    IssueType = t.IssueTypeId,
                    FullName = t.FullName,
                    Phone = t.Phone,
                    Description = t.Description,
                    Priority = t.Priority,
                    Status = t.Status,
                    TaskCreatedTime = t.ActivityLogs
                .Where(al => al.Status == (int)TicketStatus.Open)
                .Select(al => al.CreatedDate)
                .FirstOrDefault() == DateTime.MinValue
                ? "N/A"
                : t.ActivityLogs
                .Where(al => al.Status == (int)TicketStatus.Open)
                .Select(al => al.CreatedDate)
                .FirstOrDefault().ToString("yyyy-MM-dd HH:mm"),
                    TaskPickupTime = t.ActivityLogs
                .Where(al => al.Status == (int)TicketStatus.InProgress)
                .Select(al => al.CreatedDate)
                .FirstOrDefault() == DateTime.MinValue
                ? "N/A"
                : t.ActivityLogs
                .Where(al => al.Status == (int)TicketStatus.InProgress)
                .Select(al => al.CreatedDate)
                .FirstOrDefault().ToString("yyyy-MM-dd HH:mm"),

                    TaskCompletedTime = t.ActivityLogs
                .Where(al => al.Status == (int)TicketStatus.Completed)
                .Select(al => al.CreatedDate)
                .FirstOrDefault() == DateTime.MinValue
                ? "N/A"
                : t.ActivityLogs
                .Where(al => al.Status == (int)TicketStatus.Completed)
                .Select(al => al.CreatedDate)
                .FirstOrDefault().ToString("yyyy-MM-dd HH:mm"),



                    TaskClosedTime = t.ActivityLogs
                .Where(al => al.Status == (int)TicketStatus.Closed)
                .Select(al => al.CreatedDate)
                .FirstOrDefault() == DateTime.MinValue
                ? "N/A"
                : t.ActivityLogs
                .Where(al => al.Status == (int)TicketStatus.Closed)
                .Select(al => al.CreatedDate)
                .FirstOrDefault().ToString("yyyy-MM-dd HH:mm"),

                    Assignee = t.ActivityLogs
                        .Where(al => al.Status == (int)TicketStatus.InProgress)
                        .OrderByDescending(al => al.CreatedDate)
                        .Select(al => al.User.FirstName + " " + al.User.LastName)
                        .FirstOrDefault() ?? "",

                    CreatedBy = t.ActivityLogs
                        .Where(al => al.Status == (int)TicketStatus.Open)
                        .OrderByDescending(al => al.CreatedDate)
                        .Select(al => al.User.FirstName + " (" + al.User.LastName + ")")
                        .FirstOrDefault() ?? "",

                    Attachments = t.Attachments
                        .Select(a => new AttachmentFileViewModel
                        {
                            FilePath = a.FileUrl
                        }).ToList()
                })
                .FirstOrDefaultAsync();



            return ticket;
        }



    }
}
