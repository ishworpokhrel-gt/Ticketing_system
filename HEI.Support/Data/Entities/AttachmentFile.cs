namespace HEI.Support.Data.Entities
{
	public class AttachmentFile : BaseDbEntity
	{
		public string FileName { get; set; }
		public string FilePath { get; set; }
		public byte[] FileContent { get; set; }
		public string FileType { get; set; }
		public int TicketID { get; set; }
		public Ticket Ticket { get; set; }
	}
}
