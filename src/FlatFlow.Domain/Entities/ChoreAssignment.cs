using FlatFlow.Domain.Common;

namespace FlatFlow.Domain.Entities
{
    public class ChoreAssignment : BaseEntity
    {
        public Guid TenantId { get; private set; }
        public Tenant Tenant { get; private set; } = null!;

        public Guid ChoreId { get; private set; }
        public Chore Chore { get; private set; } = null!;

        public DateTime DueDate { get; private set; }
        public DateTime? CompletedAt { get; private set; }

        protected ChoreAssignment() : base() { }

        public ChoreAssignment(Guid tenantId, Guid choreId, DateTime dueDate) : base()
        {
            TenantId = tenantId;
            ChoreId = choreId;
            DueDate = dueDate;
        }

        public void UpdateDueDate(DateTime dueDate)
        {
            DueDate = dueDate;
            SetUpdatedAt();
        }

        public void Complete()
        {
            CompletedAt = DateTime.UtcNow;
            SetUpdatedAt();
        }
    }
}
