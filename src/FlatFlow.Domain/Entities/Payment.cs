using FlatFlow.Domain.Common;

namespace FlatFlow.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public string Title { get; private set; } = string.Empty;
        public decimal Amount { get; private set; }

        public Guid FlatId { get; private set; }
        public Flat Flat { get; private set; } = null!;

        public Guid CreatedById { get; private set; }
        public Tenant CreatedBy { get; private set; } = null!;

        public List<PaymentShare> PaymentShares { get; private set; } = [];

        protected Payment() : base() { }

        public Payment(string title, decimal amount, Guid flatId, Guid createdById) : base()
        {
            Title = title;
            Amount = amount;
            FlatId = flatId;
            CreatedById = createdById;
        }

        public void Update(string title, decimal amount)
        {
            Title = title;
            Amount = amount;
        }
    }
}
