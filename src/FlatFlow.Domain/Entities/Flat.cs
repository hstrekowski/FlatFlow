using FlatFlow.Domain.Common;
using FlatFlow.Domain.ValueObjects;

namespace FlatFlow.Domain.Entities
{
    public class Flat : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public Address Address { get; private set; } = null!;
        public string AccessCode { get; private set; } = string.Empty;
        public List<Tenant> Tenants { get; private set; } = [];
        public List<Chore> Chores { get; private set; } = [];
        public List<Note> Notes { get; private set; } = [];
        public List<Payment> Payments { get; private set; } = [];

        protected Flat() : base() { }
        public Flat(string name, Address address) : base()
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Flat name cannot be empty.", nameof(name));

            Name = name;
            AccessCode = GenerateAccessCode();
            Address = address ?? throw new ArgumentNullException(nameof(address));
        }

        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Flat name cannot be empty.", nameof(name));

            Name = name;
            SetUpdatedAt();
        }

        public void UpdateAddress(Address address)
        {
            Address = address ?? throw new ArgumentNullException(nameof(address));
            SetUpdatedAt();
        }

        private string GenerateAccessCode()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
        }

        public void RefreshAccessCode()
        {
            AccessCode = GenerateAccessCode();
            SetUpdatedAt();
        }
    }
}
