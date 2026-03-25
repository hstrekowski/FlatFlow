using FlatFlow.Domain.Common;
using FlatFlow.Domain.Exceptions;

namespace FlatFlow.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public string Title { get; private set; } = string.Empty;
        public decimal Amount { get; private set; }
        public DateTime DueDate { get; private set; }

        public Guid FlatId { get; private set; }
        public Flat Flat { get; private set; } = null!;

        public Guid CreatedById { get; private set; }
        public Tenant CreatedBy { get; private set; } = null!;

        private readonly List<PaymentShare> _paymentShares = [];
        public IReadOnlyList<PaymentShare> PaymentShares => _paymentShares.AsReadOnly();

        protected Payment() : base() { }

        public Payment(string title, decimal amount, DateTime dueDate, Guid flatId, Guid createdById) : base()
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainValidationException("Payment title cannot be empty.", nameof(title));
            if (amount <= 0)
                throw new DomainValidationException("Payment amount must be greater than zero.", nameof(amount));
            if (flatId == Guid.Empty)
                throw new DomainValidationException("Flat ID cannot be empty.", nameof(flatId));
            if (createdById == Guid.Empty)
                throw new DomainValidationException("Created by ID cannot be empty.", nameof(createdById));

            Title = title;
            Amount = amount;
            DueDate = dueDate;
            FlatId = flatId;
            CreatedById = createdById;
        }

        public void UpdateTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainValidationException("Payment title cannot be empty.", nameof(title));

            Title = title;
            SetUpdatedAt();
        }

        public void UpdateAmount(decimal amount)
        {
            if (amount <= 0)
                throw new DomainValidationException("Payment amount must be greater than zero.", nameof(amount));

            Amount = amount;
            SetUpdatedAt();
        }

        public void UpdateDueDate(DateTime dueDate)
        {
            DueDate = dueDate;
            SetUpdatedAt();
        }

        public PaymentShare AddShare(Guid tenantId, decimal shareAmount)
        {
            var share = new PaymentShare(tenantId, Id, shareAmount);
            _paymentShares.Add(share);
            return share;
        }

        public void RemoveShare(Guid shareId)
        {
            var share = _paymentShares.FirstOrDefault(s => s.Id == shareId)
                ?? throw new DomainException($"Payment share with ID '{shareId}' not found.");
            _paymentShares.Remove(share);
        }
    }
}
