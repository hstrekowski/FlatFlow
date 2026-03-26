using FlatFlow.Domain.Common;
using FlatFlow.Domain.Enums;
using FlatFlow.Domain.Exceptions;

namespace FlatFlow.Domain.Entities
{
    public class Chore : BaseEntity
    {
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public ChoreFrequency Frequency { get; private set; }

        public Guid FlatId { get; private set; }
        public Flat Flat { get; private set; } = null!;

        private readonly List<ChoreAssignment> _choreAssignments = [];
        public IReadOnlyList<ChoreAssignment> ChoreAssignments => _choreAssignments.AsReadOnly();

        protected Chore() : base() { }

        public Chore(string title, string description, ChoreFrequency frequency, Guid flatId) : base()
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainValidationException("Chore title cannot be empty.", nameof(title));
            if (flatId == Guid.Empty)
                throw new DomainValidationException("Flat ID cannot be empty.", nameof(flatId));

            Title = title;
            Description = description;
            Frequency = frequency;
            FlatId = flatId;
        }

        public void UpdateTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainValidationException("Chore title cannot be empty.", nameof(title));

            Title = title;
            SetUpdatedAt();
        }

        public void UpdateDescription(string description)
        {
            Description = description;
            SetUpdatedAt();
        }

        public void UpdateFrequency(ChoreFrequency frequency)
        {
            Frequency = frequency;
            SetUpdatedAt();
        }

        public ChoreAssignment AddAssignment(Guid tenantId, DateTime dueDate)
        {
            if (_choreAssignments.Any(a => a.TenantId == tenantId && !a.IsCompleted))
                throw new DomainException($"Tenant '{tenantId}' already has an active assignment for this chore.");

            var assignment = new ChoreAssignment(tenantId, Id, dueDate);
            _choreAssignments.Add(assignment);
            return assignment;
        }

        public void RemoveAssignment(Guid assignmentId)
        {
            var assignment = _choreAssignments.FirstOrDefault(a => a.Id == assignmentId)
                ?? throw new DomainException($"Chore assignment with ID '{assignmentId}' not found.");
            _choreAssignments.Remove(assignment);
        }
    }
}
