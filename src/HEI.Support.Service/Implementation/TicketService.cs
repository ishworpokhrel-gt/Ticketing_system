using HEI.Support.Common.Models;
using HEI.Support.Common.Models.Enum;
using HEI.Support.Domain.Entities;
using HEI.Support.Infrastructure.Persistence;
using HEI.Support.Infrastructure.Persistence.Repository.Interface;
using HEI.Support.Service.Interface;

namespace HEI.Support.Service.Implementation
{
    public class TicketService : ITicketService
	{
		private readonly ITicketRepository _ticketRepository;
		private readonly IAttachmentFileRepository _attachmentFileRepository;
		private readonly IActivityLogRepository _activityLogRepository;
		private readonly ApplicationDbContext _context;

        public TicketService(ITicketRepository ticketRepository, IAttachmentFileRepository attachmentFileRepository,
            IActivityLogRepository activityLogRepository, ApplicationDbContext context)
        {
            _ticketRepository = ticketRepository;
            _attachmentFileRepository = attachmentFileRepository;
            _activityLogRepository = activityLogRepository;
            _context = context;
        }


        public async Task CreateTicketAsync(TicketViewModel model, ApplicationUser user)
        {
            // Start a transaction
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Create the ticket
                    var ticket = new Ticket
                    {
                        IssueTypeId = model.IssueType,
                        Description = model.Description,
                        Priority = model.Priority,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = user.Id
                    };
                    await _ticketRepository.AddAsync(ticket);

                    // Create the activity log
                    var activityLog = new ActivityLog
                    {
                        TicketId = ticket.Id,
                        Status = TicketStatus.Open.ToString(),
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = user.Id
                    };
                    await _activityLogRepository.AddAsync(activityLog);

                    // Handle attachments if present
                    if (model.Attachments != null && model.Attachments.Any())
                    {
                        foreach (var attachment in model.Attachments)
                        {
                            var attachmentFile = new AttachmentFile
                            {
                                TicketID = ticket.Id,
                                FileName = attachment.FileName,
                                FilePath = attachment.FilePath,
                                CreatedDate = DateTime.UtcNow,
                                CreatedBy = user.Id
                            };

                            await _attachmentFileRepository.AddAsync(attachmentFile);
                        }
                    }
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw; 
                }
            }
        }

        public async Task<List<TicketViewModel>> GetAllTicketsAsync()
        {
            var data = await _ticketRepository.GetAllAsync();

            var ticketViewModels = data.Select(ticket => new TicketViewModel
            {
                IssueType = ticket.IssueTypeId,
                Description = ticket.Description,
                Priority = ticket.Priority,
                Status = ticket.Status,
            }).ToList();

            return ticketViewModels;
        }
        public Task<TicketViewModel> GetTicketByIdAsync(Guid ticketId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TicketViewModel>> GetTicketsBySupportUserIdAsync(string supportUserId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TicketViewModel>> GetTicketsByUserIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateTicketAsync(TicketViewModel model)
        {
            throw new NotImplementedException();
        }

        //public async Task<IEnumerable<TicketViewModel>> GetTicketsByUserIdAsync(string userId)
        //{
        //	return await _context.Tickets
        //		.Where(t => t.CreatedBy == userId)
        //		.Select(t => new TicketViewModel
        //		{
        //			Id = t.Id,
        //			Description = t.Description,
        //			Priority = t.Priority,
        //			Comments = t.Comments.Select(c => new CommentViewModel
        //			{
        //				Id = c.Id,
        //				Content = c.Content,

        //			}).ToList(),
        //			Attachments = t.Attachments.Select(a => new AttachmentFileViewModel
        //			{
        //				Id = a.Id,
        //				FileName = a.FileName,
        //				FilePath = a.FilePath,
        //			}).ToList(),
        //			ActivityLogs = t.ActivityLogs.Select(a => new ActivityLogViewModel
        //			{
        //				Id = a.Id,
        //				Status = a.Status,

        //			}).ToList()
        //		}).ToListAsync();
        //}

        //public async Task<IEnumerable<TicketViewModel>> GetTicketsBySupportUserIdAsync(string supportUserId)
        //{
        //	return await _context.Tickets
        //		.Where(t => t.CreatedBy == supportUserId)  // Assuming Support User is also a user and tickets are assigned by UserID
        //		.Select(t => new TicketViewModel
        //		{
        //			Id = t.Id,
        //			Description = t.Description,
        //			Priority = t.Priority,

        //			Comments = t.Comments.Select(c => new CommentViewModel
        //			{
        //				Id = c.Id,
        //				Content = c.Content,

        //			}).ToList(),
        //			Attachments = t.Attachments.Select(a => new AttachmentFileViewModel
        //			{
        //				Id = a.Id,
        //				FileName = a.FileName,
        //				FilePath = a.FilePath,
        //			}).ToList(),
        //			ActivityLogs = t.ActivityLogs.Select(a => new ActivityLogViewModel
        //			{
        //				Id = a.Id,

        //			}).ToList()
        //		}).ToListAsync();
        //}

        //public async Task<TicketViewModel> GetTicketByIdAsync(Guid ticketId)
        //{
        //	var ticket = await _context.Tickets
        //		.Where(t => t.Id == ticketId)
        //		.Select(t => new TicketViewModel
        //		{
        //			Id = t.Id,
        //			Description = t.Description,
        //			Priority = t.Priority,
        //			Comments = t.Comments.Select(c => new CommentViewModel
        //			{
        //				Id = c.Id,
        //				Content = c.Content,
        //			}).ToList(),
        //			Attachments = t.Attachments.Select(a => new AttachmentFileViewModel
        //			{
        //				Id = a.Id,
        //				FileName = a.FileName,
        //				FilePath = a.FilePath,
        //			}).ToList(),
        //			ActivityLogs = t.ActivityLogs.Select(a => new ActivityLogViewModel
        //			{
        //				Id = a.Id
        //			}).ToList()
        //		}).FirstOrDefaultAsync();

        //	return ticket;
        //}

        //public async Task UpdateTicketAsync(TicketViewModel model)
        //{
        //	var ticket = await _context.Tickets.FindAsync(model.Id);
        //	if (ticket != null)
        //	{
        //		ticket.Description = model.Description;
        //		ticket.Priority = model.Priority;
        //		// Optionally update related entities here

        //		_context.Tickets.Update(ticket);
        //		await _context.SaveChangesAsync();
        //	}
        //}
    }
}
