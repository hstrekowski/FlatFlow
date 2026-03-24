namespace FlatFlow.Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; }
        public DateTime CreatedAt { get; protected set; }

        protected BaseEntity()
        { 
            this.Id = Guid.NewGuid();
            this.CreatedAt = DateTime.UtcNow;
        }
        
    }
}
