namespace HEI.Support.Domain.Entities
{
	public class ActivityLog : BaseDbEntity
	{
		public int Status { get; set; }  // e.g., "Open", "InProgress", "Completed", "Closed"
		public Guid TicketId { get; set; }
		public Ticket Ticket { get; set; }
	}
}
