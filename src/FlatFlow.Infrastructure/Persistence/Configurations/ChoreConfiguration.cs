using FlatFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlatFlow.Infrastructure.Persistence.Configurations
{
    public class ChoreConfiguration : IEntityTypeConfiguration<Chore>
    {
        public void Configure(EntityTypeBuilder<Chore> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Description)
                .HasMaxLength(1000);

            builder.Property(c => c.Frequency)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(20);

            builder.HasOne(c => c.Flat)
                .WithMany(f => f.Chores)
                .HasForeignKey(c => c.FlatId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.ChoreAssignments)
                .WithOne(ca => ca.Chore)
                .HasForeignKey(ca => ca.ChoreId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(c => c.ChoreAssignments).UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
