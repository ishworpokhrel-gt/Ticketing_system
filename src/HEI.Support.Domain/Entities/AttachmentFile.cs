namespace HEI.Support.Domain.Entities
{
	public class AttachmentFile : BaseDbEntity
	{
		public string FileName { get; set; }
		public string FilePath { get; set; }
		public string FileType { get; set; }
		public Guid TicketID { get; set; }
		public Ticket Ticket { get; set; }
	}
}
