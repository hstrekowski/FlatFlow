using FlatFlow.Domain.Common;

namespace FlatFlow.Domain.Entities
{
    public class ChoreAssignment : BaseEntity
    {
        public Guid TenantId { get; private set; }
        public Tenant Tenant { get; private set; } = null!;

        public Guid ChoreId { get; private set; }
        public Chore Chore { get; private set; } = null!;

        public DateTime? CompletedAt { get; private set; }

        protected ChoreAssignment() : base() { }

        public ChoreAssignment(Guid tenantId, Guid choreId) : base()
        {
            TenantId = tenantId;
            ChoreId = choreId;
        }

        public void Complete()
        {
            CompletedAt = DateTime.UtcNow;
        }
    }
}
