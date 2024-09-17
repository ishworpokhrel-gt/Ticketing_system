using HEI.Support.Data.Entities;
using HEI.Support.Data;
using HEI.Support.Models;
using HEI.Support.Service.Interface;

namespace HEI.Support.Service.Implementation
{
    public class TicketService //: ITicketService
	{
		private readonly ApplicationDbContext _context;

		public TicketService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task CreateTicketAsync(TicketViewModel model, string userId)
		{
			var ticket = new Ticket
			{
				Subject = model.Subject,
				Description = model.Description,
				Priority = model.Priority,
				Status = model.Status,
				CreatedDate = DateTime.UtcNow
			};
			var activityLog = new ActivityLog
			{
				Action = "Created",
				CreatedDate = DateTime.UtcNow,
			};
			ticket.ActivityLogs.Add(activityLog);

			if (model.Attachments != null && model.Attachments.Any())
			{
				foreach (var attachment in model.Attachments)
				{
					var attachmentFile = new AttachmentFile
					{
						Id = Guid.NewGuid(),
						FileName = attachment.FileName,
						FilePath = attachment.FilePath,
						CreatedDate = DateTime.UtcNow,
						//CreatedBy = userId
					};
					ticket.Attachments.Add(attachmentFile);
				}
			}

			if (model.Comments != null && model.Comments.Any())
			{
				foreach (var comment in model.Comments)
				{
					var ticketComment = new Comment
					{
						Id = Guid.NewGuid(),
						Content = comment.Content,
						//CreatedDate = comment.CreatedDate,
						//CreatedBy = userId
					};
					ticket.Comments.Add(ticketComment);
				}
			}

			//_context.Tickets.Add(ticket);
			await _context.SaveChangesAsync();
		}

		//public async Task<IEnumerable<TicketViewModel>> GetTicketsByUserIdAsync(string userId)
		//{
		//	return await _context.Tickets
		//		.Where(t => t.UserID == userId)
		//		.Select(t => new TicketViewModel
		//		{
		//			Id = t.Id,
		//			Subject = t.Subject,
		//			Description = t.Description,
		//			Priority = t.Priority,
		//			Status = t.Status,
		//			CreatedDate = t.CreatedDate,
		//			ResolvedDate = t.ResolvedDate,
		//			UserID = t.UserID,
		//			Comments = t.Comments.Select(c => new CommentViewModel
		//			{
		//				Id = c.Id,
		//				Content = c.Content,
		//				CreatedDate = c.CreatedDate,
		//				UserID = c.UserID
		//			}).ToList(),
		//			Attachments = t.Attachments.Select(a => new AttachmentFileViewModel
		//			{
		//				Id = a.Id,
		//				FileName = a.FileName,
		//				FilePath = a.FilePath,
		//				CreatedDate = a.CreatedDate
		//			}).ToList(),
		//			ActivityLogs = t.ActivityLogs.Select(a => new ActivityLogViewModel
		//			{
		//				Id = a.Id,
		//				Action = a.Action,
		//				CreatedDate = a.CreatedDate,
		//				UserID = a.UserID
		//			}).ToList()
		//		}).ToListAsync();
		//}

		//public async Task<IEnumerable<TicketViewModel>> GetTicketsBySupportUserIdAsync(string supportUserId)
		//{
		//	return await _context.Tickets
		//		.Where(t => t.UserID == supportUserId)  // Assuming Support User is also a user and tickets are assigned by UserID
		//		.Select(t => new TicketViewModel
		//		{
		//			Id = t.Id,
		//			Subject = t.Subject,
		//			Description = t.Description,
		//			Priority = t.Priority,
		//			Status = t.Status,
		//			CreatedDate = t.CreatedDate,
		//			ResolvedDate = t.ResolvedDate,
		//			UserID = t.UserID,
		//			Comments = t.Comments.Select(c => new CommentViewModel
		//			{
		//				Id = c.Id,
		//				Content = c.Content,
		//				CreatedDate = c.CreatedDate,
		//				UserID = c.UserID
		//			}).ToList(),
		//			Attachments = t.Attachments.Select(a => new AttachmentFileViewModel
		//			{
		//				Id = a.Id,
		//				FileName = a.FileName,
		//				FilePath = a.FilePath,
		//				CreatedDate = a.CreatedDate
		//			}).ToList(),
		//			ActivityLogs = t.ActivityLogs.Select(a => new ActivityLogViewModel
		//			{
		//				Id = a.Id,
		//				Action = a.Action,
		//				CreatedDate = a.CreatedDate,
		//				UserID = a.UserID
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
		//			Subject = t.Subject,
		//			Description = t.Description,
		//			Priority = t.Priority,
		//			Status = t.Status,
		//			CreatedDate = t.CreatedDate,
		//			ResolvedDate = t.ResolvedDate,
		//			UserID = t.UserID,
		//			Comments = t.Comments.Select(c => new CommentViewModel
		//			{
		//				Id = c.Id,
		//				Content = c.Content,
		//				CreatedDate = c.CreatedDate,
		//				UserID = c.UserID
		//			}).ToList(),
		//			Attachments = t.Attachments.Select(a => new AttachmentFileViewModel
		//			{
		//				Id = a.Id,
		//				FileName = a.FileName,
		//				FilePath = a.FilePath,
		//				CreatedDate = a.CreatedDate
		//			}).ToList(),
		//			ActivityLogs = t.ActivityLogs.Select(a => new ActivityLogViewModel
		//			{
		//				Id = a.Id,
		//				Action = a.Action,
		//				CreatedDate = a.CreatedDate,
		//				UserID = a.UserID
		//			}).ToList()
		//		}).FirstOrDefaultAsync();

		//	return ticket;
		//}

		//public async Task UpdateTicketAsync(TicketViewModel model)
		//{
		//	var ticket = await _context.Tickets.FindAsync(model.Id);
		//	if (ticket != null)
		//	{
		//		ticket.Subject = model.Subject;
		//		ticket.Description = model.Description;
		//		ticket.Priority = model.Priority;
		//		ticket.Status = model.Status;
		//		ticket.ResolvedDate = model.ResolvedDate;

		//		// Optionally update related entities here

		//		_context.Tickets.Update(ticket);
		//		await _context.SaveChangesAsync();
		//	}
		//}
	}
}
