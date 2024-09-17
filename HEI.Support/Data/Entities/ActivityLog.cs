namespace HEI.Support.Data.Entities
{
	public class ActivityLog : BaseDbEntity
	{
		public string Action { get; set; }  // e.g., "Created", "Updated", "Closed","Resolved"
		public DateTime ActionDate { get; set; }
		public Guid TicketId { get; set; }
		public Ticket Ticket { get; set; }
	}
}
