using HEI.Support.Data.Interface;

namespace HEI.Support.Data.Entities
{
	public class BaseDbEntity : ISoftDeletableEntity
	{
		public Guid Id { get; set; }
		public Guid CreatedBy { get; set; }
		public int UpdatedBy { get; set; }
		public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
		public DateTime UpdatedDate { get; set; }
		public bool IsDeleted { get; set; }
	}
}
