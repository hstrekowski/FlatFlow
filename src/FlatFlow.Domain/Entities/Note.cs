using FlatFlow.Domain.Common;

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
            Title = title;
            Content = content;
            FlatId = flatId;
            AuthorId = authorId;
        }

        public void UpdateContent(string title, string content)
        {
            Title = title;
            Content = content;
            SetUpdatedAt();
        }
    }
}
