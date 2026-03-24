using FlatFlow.Domain.Common;
using FlatFlow.Domain.Exceptions;

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
        public List<Payment> CreatedPayments { get; private set; } = [];
        public List<Note> AuthoredNotes { get; private set; } = [];

        protected Tenant() : base() { }

        public Tenant(string firstName, string lastName, string email, string userId, Guid flatId, bool isOwner = false) : base()
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new DomainValidationException("First name cannot be empty.", nameof(firstName));
            if (string.IsNullOrWhiteSpace(lastName))
                throw new DomainValidationException("Last name cannot be empty.", nameof(lastName));
            if (string.IsNullOrWhiteSpace(email))
                throw new DomainValidationException("Email cannot be empty.", nameof(email));
            if (string.IsNullOrWhiteSpace(userId))
                throw new DomainValidationException("User ID cannot be empty.", nameof(userId));
            if (flatId == Guid.Empty)
                throw new DomainValidationException("Flat ID cannot be empty.", nameof(flatId));

            FirstName = firstName;
            LastName = lastName;
            Email = email;
            UserId = userId;
            FlatId = flatId;
            IsOwner = isOwner;
        }

        public void UpdateProfile(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new DomainValidationException("First name cannot be empty.", nameof(firstName));
            if (string.IsNullOrWhiteSpace(lastName))
                throw new DomainValidationException("Last name cannot be empty.", nameof(lastName));

            FirstName = firstName;
            LastName = lastName;
            SetUpdatedAt();
        }

        public void UpdateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new DomainValidationException("Email cannot be empty.", nameof(email));

            Email = email;
            SetUpdatedAt();
        }

        public void PromoteToOwner()
        {
            if (IsOwner)
                throw new DomainException("Tenant is already an owner.");

            IsOwner = true;
            SetUpdatedAt();
        }

        public void RevokeOwnership()
        {
            if (!IsOwner)
                throw new DomainException("Tenant is not an owner.");

            IsOwner = false;
            SetUpdatedAt();
        }
    }
}
