using FlatFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlatFlow.Infrastructure.Persistence.Configurations
{
    public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
    {
        public void Configure(EntityTypeBuilder<Tenant> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Email)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.UserId)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasIndex(t => t.UserId);

            builder.HasIndex(t => new { t.FlatId, t.UserId })
                .IsUnique();

            builder.HasOne(t => t.Flat)
                .WithMany(f => f.Tenants)
                .HasForeignKey(t => t.FlatId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(t => t.ChoreAssignments)
                .WithOne(ca => ca.Tenant)
                .HasForeignKey(ca => ca.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(t => t.PaymentShares)
                .WithOne(ps => ps.Tenant)
                .HasForeignKey(ps => ps.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(t => t.CreatedPayments)
                .WithOne(p => p.CreatedBy)
                .HasForeignKey(p => p.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(t => t.AuthoredNotes)
                .WithOne(n => n.Author)
                .HasForeignKey(n => n.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Navigation(t => t.ChoreAssignments).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(t => t.PaymentShares).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(t => t.CreatedPayments).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(t => t.AuthoredNotes).UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
