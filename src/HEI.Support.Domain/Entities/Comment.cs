﻿namespace HEI.Support.Domain.Entities
{
	public class Comment : BaseDbEntity
	{
		public string Content { get; set; }
		public Guid TicketID { get; set; }
		public Ticket Ticket { get; set; }
	}
}
