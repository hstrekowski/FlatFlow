using FlatFlow.Domain.Common;

namespace FlatFlow.Domain.Entities
{
    public class Tenant : BaseEntity
    {
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string UserId { get; private set; } = string.Empty;
        public bool IsOwner { get; private set; }

        public Guid FlatId { get; private set; }
        public Flat Flat { get; private set; } = null!;

        public List<ChoreAssignment> ChoreAssignments { get; private set; } = [];
        public List<PaymentShare> PaymentShares { get; private set; } = [];

        protected Tenant() : base() { }

        public Tenant(string firstName, string lastName, string email, string userId, Guid flatId, bool isOwner = false) : base()
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            UserId = userId;
            FlatId = flatId;
            IsOwner = isOwner;
        }

        public void UpdateProfile(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public void UpdateEmail(string email)
        {
            Email = email;
        }
    }
}
