using FlatFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlatFlow.Infrastructure.Persistence.Configurations
{
    public class NoteConfiguration : IEntityTypeConfiguration<Note>
    {
        public void Configure(EntityTypeBuilder<Note> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(n => n.Content)
                .HasMaxLength(5000);

            builder.HasOne(n => n.Flat)
                .WithMany(f => f.Notes)
                .HasForeignKey(n => n.FlatId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(n => n.Author)
                .WithMany(t => t.AuthoredNotes)
                .HasForeignKey(n => n.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
