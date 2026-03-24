using FlatFlow.Domain.Common;
using FlatFlow.Domain.Enums;

namespace FlatFlow.Domain.Entities
{
    public class PaymentShare : BaseEntity
    {
        public Guid TenantId { get; private set; }
        public Tenant Tenant { get; private set; } = null!;

        public Guid PaymentId { get; private set; }
        public Payment Payment { get; private set; } = null!;

        public decimal ShareAmount { get; private set; }
        public PaymentShareStatus Status { get; private set; }

        protected PaymentShare() : base() { }

        public PaymentShare(Guid tenantId, Guid paymentId, decimal shareAmount) : base()
        {
            TenantId = tenantId;
            PaymentId = paymentId;
            ShareAmount = shareAmount;
            Status = PaymentShareStatus.New;
        }

        public void MarkAsPartial()
        {
            Status = PaymentShareStatus.Partial;
            SetUpdatedAt();
        }

        public void MarkAsPaid()
        {
            Status = PaymentShareStatus.Paid;
            SetUpdatedAt();
        }
    }
}
