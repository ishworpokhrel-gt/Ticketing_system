namespace HEI.Support.Domain.Entities
{
	public class AttachmentFile : BaseDbEntity
	{
		public string FileUrl { get; set; }
		public string FileType { get; set; }
		public Guid TicketID { get; set; }
		public Ticket Ticket { get; set; }
	}
}
