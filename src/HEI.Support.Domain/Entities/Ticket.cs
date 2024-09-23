namespace HEI.Support.Domain.Entities
{
	public class Ticket : BaseDbEntity
	{
        public int IssueTypeId { get; set; }
        public string Description { get; set; }
		public int Priority { get; set; }
        public int Status { get; set; }
        public ICollection<Comment> Comments { get; set; }
		public ICollection<AttachmentFile> Attachments { get; set; }
		public ICollection<ActivityLog> ActivityLogs { get; set; }
	}
}
