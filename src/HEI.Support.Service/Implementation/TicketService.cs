using HEI.Support.Common.Models;
using HEI.Support.Common.Models.Enum;
using HEI.Support.Domain.Entities;
using HEI.Support.Infrastructure.Persistence;
using HEI.Support.Infrastructure.Persistence.Repository.Interface;
using HEI.Support.Service.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Net.Mail;

namespace HEI.Support.Service.Implementation
{
    public class TicketService : ITicketService
	{
		private readonly ITicketRepository _ticketRepository;
		private readonly IAttachmentFileRepository _attachmentFileRepository;
		private readonly IActivityLogRepository _activityLogRepository;
		private readonly ApplicationDbContext _context;
        private IWebHostEnvironment _webHostEnvironment;

        public TicketService(ITicketRepository ticketRepository, IAttachmentFileRepository attachmentFileRepository,
            IActivityLogRepository activityLogRepository, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _ticketRepository = ticketRepository;
            _attachmentFileRepository = attachmentFileRepository;
            _activityLogRepository = activityLogRepository;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
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
                        FullName = model.FullName,
                        Phone = model.Phone,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = user.Id
                    };
                    await _ticketRepository.AddAsync(ticket);

                    // Create the activity log
                    var activityLog = new ActivityLog
                    {
                        TicketId = ticket.Id,
                        Status = ticket.Status,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = user.Id
                    };
                    await _activityLogRepository.AddAsync(activityLog);
                    var attachedFiles = await UploadImageAsync(model.Attachment);

                    var userUploadItems = attachedFiles.Select(uploadedFileName => new AttachmentFile()
                    {
                        TicketID = ticket.Id,
                        FileUrl = uploadedFileName,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = user.Id
                    }).ToList();
                   await _attachmentFileRepository.AddMultipleEntity(userUploadItems);

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

        public async Task <List<string>> UploadImageAsync(List<IFormFile> files)
        {
            var uniqueFileName = new List<string>();
            var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "Ticket");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            foreach (var item in files)
            {
                var renamedFileName = $"{DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}{DateTime.Now.Hour}{DateTime.Now.Minute}{DateTime.Now.Second}{DateTime.Now.Millisecond}";

                var fileName = $"{renamedFileName}{Path.GetExtension(item.FileName)}";

                await using var stream = new FileStream(Path.Combine(folderPath, fileName), FileMode.Create);

                await item.CopyToAsync(stream);

                uniqueFileName.Add(Path.Combine("Ticket", fileName));
            }
            return uniqueFileName;
        }
        public async Task<TicketViewModel> GetTicketByIdAsync(Guid ticketId)

        {
            var ticket = await _ticketRepository.GetAsync(ticketId);
            if (ticket == null)
            {
                throw new KeyNotFoundException($"Ticket with Id {ticketId} not found.");
            }

            var result = new TicketViewModel
            {
                Id = ticket.Id,
                IssueType = ticket.IssueTypeId, 
                Description = ticket.Description,
                Priority = ticket.Priority,
                Status = ticket.Status,
                Attachments = ticket.Attachments?.Select(a => new AttachmentFileViewModel
                {
                    FilePath = a.FileUrl
                    
                }).ToList(),
                Comments = ticket.Comments?.Select(c => new CommentViewModel
                {
                   Content = c.Content,
                }).ToList(),
                ActivityLogs = ticket.ActivityLogs?.Select(l => new ActivityLogViewModel
                {
                   Status = ticket.Status
                }).ToList()
            };

            return result;

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
