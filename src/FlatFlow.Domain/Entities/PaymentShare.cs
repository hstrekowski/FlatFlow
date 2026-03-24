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
            if (tenantId == Guid.Empty)
                throw new ArgumentException("Tenant ID cannot be empty.", nameof(tenantId));
            if (paymentId == Guid.Empty)
                throw new ArgumentException("Payment ID cannot be empty.", nameof(paymentId));
            if (shareAmount <= 0)
                throw new ArgumentException("Share amount must be greater than zero.", nameof(shareAmount));

            TenantId = tenantId;
            PaymentId = paymentId;
            ShareAmount = shareAmount;
            Status = PaymentShareStatus.New;
        }

        public void MarkAsPartial()
        {
            if (Status == PaymentShareStatus.Paid)
                throw new InvalidOperationException("Cannot mark a paid share as partial.");

            Status = PaymentShareStatus.Partial;
            SetUpdatedAt();
        }

        public void MarkAsPaid()
        {
            if (Status == PaymentShareStatus.Paid)
                throw new InvalidOperationException("Payment share is already paid.");

            Status = PaymentShareStatus.Paid;
            SetUpdatedAt();
        }
    }
}
