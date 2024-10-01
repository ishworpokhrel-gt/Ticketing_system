using HEI.Support.Common.Models.Enum;
using Microsoft.AspNetCore.Http;
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
        public string FullName { get; set; }
        public string Phone { get; set; }
        public int Status { get; set; }
        public string? CreatedBy { get; set; }
        public string? ReportedbBy { get; set; }
        public string? Assignee { get; set; }
        public string? AsignTo { get; set; }
        public List<IFormFile>? Attachment { get; set; }

        public List<CommentViewModel>? Comments { get; set; } = new List<CommentViewModel>();
		public List<AttachmentFileViewModel>? Attachments { get; set; } = new List<AttachmentFileViewModel>();
		public List<ActivityLogViewModel>? ActivityLogs { get; set; } = new List<ActivityLogViewModel>();
        public string?   TaskCreatedTime { get; set; }
        public string? TaskPickupTime { get; set; }
        public string? TaskCompletedTime { get; set; }
        public string? TaskClosedTime { get; set; }
        public string? TotalTimeTaken { get; set; }
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
