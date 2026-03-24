using FlatFlow.Domain.Common;
using FlatFlow.Domain.Enums;

namespace FlatFlow.Domain.Entities
{
    public class Chore : BaseEntity
    {
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public ChoreFrequency Frequency { get; private set; }

        public Guid FlatId { get; private set; }
        public Flat Flat { get; private set; } = null!;

        public List<ChoreAssignment> ChoreAssignments { get; private set; } = [];

        protected Chore() : base() { }

        public Chore(string title, string description, ChoreFrequency frequency, Guid flatId) : base()
        {
            Title = title;
            Description = description;
            Frequency = frequency;
            FlatId = flatId;
        }

        public void Update(string title, string description, ChoreFrequency frequency)
        {
            Title = title;
            Description = description;
            Frequency = frequency;
        }
    }
}
