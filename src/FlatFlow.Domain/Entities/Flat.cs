using FlatFlow.Domain.Common;
using FlatFlow.Domain.Enums;
using FlatFlow.Domain.Exceptions;
using FlatFlow.Domain.ValueObjects;

namespace FlatFlow.Domain.Entities
{
    public class Flat : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public Address Address { get; private set; } = null!;
        public string AccessCode { get; private set; } = string.Empty;

        private readonly List<Tenant> _tenants = [];
        public IReadOnlyList<Tenant> Tenants => _tenants.AsReadOnly();

        private readonly List<Chore> _chores = [];
        public IReadOnlyList<Chore> Chores => _chores.AsReadOnly();

        private readonly List<Note> _notes = [];
        public IReadOnlyList<Note> Notes => _notes.AsReadOnly();

        private readonly List<Payment> _payments = [];
        public IReadOnlyList<Payment> Payments => _payments.AsReadOnly();

        protected Flat() : base() { }
        public Flat(string name, Address address) : base()
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainValidationException("Flat name cannot be empty.", nameof(name));
            if (address is null)
                throw new DomainValidationException("Address cannot be null.", nameof(address));

            Name = name;
            AccessCode = GenerateAccessCode();
            Address = address;
        }

        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainValidationException("Flat name cannot be empty.", nameof(name));

            Name = name;
            SetUpdatedAt();
        }

        public void UpdateAddress(Address address)
        {
            if (address is null)
                throw new DomainValidationException("Address cannot be null.", nameof(address));

            Address = address;
            SetUpdatedAt();
        }

        private const string AccessCodeCharacters = "ABCDEFGHJKMNPQRSTUVWXYZ2345679";
        private const int AccessCodeLength = 8;

        private static string GenerateAccessCode()
        {
            return string.Create(AccessCodeLength, AccessCodeCharacters, (span, chars) =>
            {
                var random = Random.Shared;
                for (var i = 0; i < span.Length; i++)
                {
                    span[i] = chars[random.Next(chars.Length)];
                }
            });
        }

        public void RefreshAccessCode()
        {
            AccessCode = GenerateAccessCode();
            SetUpdatedAt();
        }

        public Tenant AddTenant(string firstName, string lastName, string email, string userId, bool isOwner = false)
        {
            if (_tenants.Any(t => t.UserId == userId))
                throw new DomainException($"User '{userId}' is already a tenant in this flat.");

            var tenant = new Tenant(firstName, lastName, email, userId, Id, isOwner);
            _tenants.Add(tenant);
            return tenant;
        }

        public void RemoveTenant(Guid tenantId)
        {
            var tenant = _tenants.FirstOrDefault(t => t.Id == tenantId)
                ?? throw new DomainException($"Tenant with ID '{tenantId}' not found.");
            _tenants.Remove(tenant);
        }

        public Chore AddChore(string title, string description, ChoreFrequency frequency, Guid createdById)
        {
            var chore = new Chore(title, description, frequency, Id, createdById);
            _chores.Add(chore);
            return chore;
        }

        public void RemoveChore(Guid choreId)
        {
            var chore = _chores.FirstOrDefault(c => c.Id == choreId)
                ?? throw new DomainException($"Chore with ID '{choreId}' not found.");
            _chores.Remove(chore);
        }

        public Note AddNote(string title, string content, Guid authorId)
        {
            var note = new Note(title, content, Id, authorId);
            _notes.Add(note);
            return note;
        }

        public void RemoveNote(Guid noteId)
        {
            var note = _notes.FirstOrDefault(n => n.Id == noteId)
                ?? throw new DomainException($"Note with ID '{noteId}' not found.");
            _notes.Remove(note);
        }

        public Payment AddPayment(string title, decimal amount, DateTime dueDate, Guid createdById)
        {
            var payment = new Payment(title, amount, dueDate, Id, createdById);
            _payments.Add(payment);
            return payment;
        }

        public void RemovePayment(Guid paymentId)
        {
            var payment = _payments.FirstOrDefault(p => p.Id == paymentId)
                ?? throw new DomainException($"Payment with ID '{paymentId}' not found.");
            _payments.Remove(payment);
        }
    }
}
