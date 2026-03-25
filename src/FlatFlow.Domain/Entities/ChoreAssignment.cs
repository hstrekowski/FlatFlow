using FlatFlow.Domain.Common;
using FlatFlow.Domain.Exceptions;

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
            if (tenantId == Guid.Empty)
                throw new DomainValidationException("Tenant ID cannot be empty.", nameof(tenantId));
            if (choreId == Guid.Empty)
                throw new DomainValidationException("Chore ID cannot be empty.", nameof(choreId));

            TenantId = tenantId;
            ChoreId = choreId;
            DueDate = dueDate;
        }

        public void UpdateDueDate(DateTime dueDate)
        {
            DueDate = dueDate;
            SetUpdatedAt();
        }

        public bool IsCompleted => CompletedAt.HasValue;

        public void Complete()
        {
            if (IsCompleted)
                throw new DomainException("Chore assignment is already completed.");

            CompletedAt = DateTime.UtcNow;
            SetUpdatedAt();
        }

        public void Reopen()
        {
            if (!IsCompleted)
                throw new DomainException("Chore assignment is not completed.");

            CompletedAt = null;
            SetUpdatedAt();
        }
    }
}
