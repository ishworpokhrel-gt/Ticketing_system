using HEI.Support.Common.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace HEI.Support.Common.Models
{
	public class TicketViewModel
	{
		public Guid Id { get; set; }
		[Display(Name = "Issue Type")]
		public int IssueType { get; set; }
		public string Description { get; set; }
		public int Priority { get; set; }
		public int Status { get; set; }
		public List<CommentViewModel>? Comments { get; set; } = new List<CommentViewModel>();
		public List<AttachmentFileViewModel>? Attachments { get; set; } = new List<AttachmentFileViewModel>();
		public List<ActivityLogViewModel>? ActivityLogs { get; set; } = new List<ActivityLogViewModel>();
		public string PriorityValue
		{
			get
			{
				return ((Priority)Priority).ToString();
			}
		}
		public string StatusValue
		{
			get
			{
				return ((TicketStatus)Status).ToString();
			}
		}
		public string IssueTypeValue
		{
			get
			{
				return ((IssueType)IssueType).ToString();
			}
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
}
