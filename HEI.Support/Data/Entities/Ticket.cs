using System.Net.Mail;

namespace HEI.Support.Data.Entities
{
	public class Ticket : BaseDbEntity
	{
		public string Subject { get; set; }
		public string Description { get; set; }
		public int Priority { get; set; }
		public DateTime? ResolvedDate { get; set; }
		public string Status { get; set; }
		public ICollection<Comment> Comments { get; set; }
		public ICollection<AttachmentFile> Attachments { get; set; }
		public ICollection<ActivityLog> ActivityLogs { get; set; }
	}
}
