using FlatFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlatFlow.Infrastructure.Persistence.Configurations
{
    public class ChoreAssignmentConfiguration : IEntityTypeConfiguration<ChoreAssignment>
    {
        public void Configure(EntityTypeBuilder<ChoreAssignment> builder)
        {
            builder.HasKey(ca => ca.Id);

            builder.Property(ca => ca.DueDate)
                .IsRequired();

            builder.HasOne(ca => ca.Chore)
                .WithMany(c => c.ChoreAssignments)
                .HasForeignKey(ca => ca.ChoreId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ca => ca.Tenant)
                .WithMany(t => t.ChoreAssignments)
                .HasForeignKey(ca => ca.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(ca => ca.IsCompleted);
        }
    }
}
