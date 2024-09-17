namespace HEI.Support.Models
{
	public class TicketViewModel
	{
		public Guid Id { get; set; }
		public string Subject { get; set; }
		public string Description { get; set; }
		public int Priority { get; set; }
		public string Status { get; set; }
		public DateTime? ResolvedDate { get; set; }
		public List<CommentViewModel> Comments { get; set; } = new List<CommentViewModel>();
		public List<AttachmentFileViewModel> Attachments { get; set; } = new List<AttachmentFileViewModel>();
		public List<ActivityLogViewModel> ActivityLogs { get; set; } = new List<ActivityLogViewModel>();
	}
	public class TicketDetailsViewModel
	{
		public TicketViewModel Ticket { get; set; }
	}
	public class TicketListViewModel
	{
		public IEnumerable<TicketViewModel> Tickets { get; set; }
	}
}
