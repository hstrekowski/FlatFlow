using FlatFlow.Domain.Common;
using FlatFlow.Domain.Exceptions;

namespace FlatFlow.Domain.Entities
{
    public class Note : BaseEntity
    {
        public string Title { get; private set; } = string.Empty;
        public string Content { get; private set; } = string.Empty;

        public Guid FlatId { get; private set; }
        public Flat Flat { get; private set; } = null!;

        public Guid AuthorId { get; private set; }
        public Tenant Author { get; private set; } = null!;

        protected Note() : base() { }

        public Note(string title, string content, Guid flatId, Guid authorId) : base()
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainValidationException("Note title cannot be empty.", nameof(title));
            if (flatId == Guid.Empty)
                throw new DomainValidationException("Flat ID cannot be empty.", nameof(flatId));
            if (authorId == Guid.Empty)
                throw new DomainValidationException("Author ID cannot be empty.", nameof(authorId));

            Title = title;
            Content = content;
            FlatId = flatId;
            AuthorId = authorId;
        }

        public void UpdateTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainValidationException("Note title cannot be empty.", nameof(title));

            Title = title;
            SetUpdatedAt();
        }

        public void UpdateContent(string content)
        {
            Content = content;
            SetUpdatedAt();
        }
    }
}
