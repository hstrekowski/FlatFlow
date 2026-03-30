using FlatFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlatFlow.Infrastructure.Persistence.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Amount)
                .HasPrecision(18, 2);

            builder.Property(p => p.DueDate)
                .IsRequired();

            builder.HasOne(p => p.Flat)
                .WithMany(f => f.Payments)
                .HasForeignKey(p => p.FlatId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.CreatedBy)
                .WithMany(t => t.CreatedPayments)
                .HasForeignKey(p => p.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.PaymentShares)
                .WithOne(ps => ps.Payment)
                .HasForeignKey(ps => ps.PaymentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(p => p.PaymentShares).UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
