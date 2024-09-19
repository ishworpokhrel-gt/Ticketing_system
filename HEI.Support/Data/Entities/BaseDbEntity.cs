using HEI.Support.Data.Interface;

namespace HEI.Support.Data.Entities
{
	public class BaseDbEntity
	{
		public Guid Id { get; set; }
		public string CreatedBy { get; set; }
		public string? UpdatedBy { get; set; }
		public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
		public DateTime? UpdatedDate { get; set; }
		public bool IsDeleted { get; set; }
	}
}
