namespace HEI.Support.Data.Entities
{
	public class ActivityLog : BaseDbEntity
	{
		public string Status { get; set; }  // e.g., "Open", "InProgress", "Completed", "Closed"
		public Guid TicketId { get; set; }
		public Ticket Ticket { get; set; }
	}
}
