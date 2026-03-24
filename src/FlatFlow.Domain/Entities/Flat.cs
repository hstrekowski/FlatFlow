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
        public List<Tenant> Tenants { get; private set; } = [];
        public List<Chore> Chores { get; private set; } = [];
        public List<Note> Notes { get; private set; } = [];
        public List<Payment> Payments { get; private set; } = [];

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

        private string GenerateAccessCode()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
        }

        public void RefreshAccessCode()
        {
            AccessCode = GenerateAccessCode();
            SetUpdatedAt();
        }

        public Tenant AddTenant(string firstName, string lastName, string email, string userId, bool isOwner = false)
        {
            var tenant = new Tenant(firstName, lastName, email, userId, Id, isOwner);
            Tenants.Add(tenant);
            return tenant;
        }

        public void RemoveTenant(Guid tenantId)
        {
            var tenant = Tenants.FirstOrDefault(t => t.Id == tenantId)
                ?? throw new DomainException($"Tenant with ID '{tenantId}' not found.");
            Tenants.Remove(tenant);
        }

        public Chore AddChore(string title, string description, ChoreFrequency frequency)
        {
            var chore = new Chore(title, description, frequency, Id);
            Chores.Add(chore);
            return chore;
        }

        public void RemoveChore(Guid choreId)
        {
            var chore = Chores.FirstOrDefault(c => c.Id == choreId)
                ?? throw new DomainException($"Chore with ID '{choreId}' not found.");
            Chores.Remove(chore);
        }

        public Note AddNote(string title, string content, Guid authorId)
        {
            var note = new Note(title, content, Id, authorId);
            Notes.Add(note);
            return note;
        }

        public void RemoveNote(Guid noteId)
        {
            var note = Notes.FirstOrDefault(n => n.Id == noteId)
                ?? throw new DomainException($"Note with ID '{noteId}' not found.");
            Notes.Remove(note);
        }

        public Payment AddPayment(string title, decimal amount, DateTime dueDate, Guid createdById)
        {
            var payment = new Payment(title, amount, dueDate, Id, createdById);
            Payments.Add(payment);
            return payment;
        }

        public void RemovePayment(Guid paymentId)
        {
            var payment = Payments.FirstOrDefault(p => p.Id == paymentId)
                ?? throw new DomainException($"Payment with ID '{paymentId}' not found.");
            Payments.Remove(payment);
        }
    }
}
