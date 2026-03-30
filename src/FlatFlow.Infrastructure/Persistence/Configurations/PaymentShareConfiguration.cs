using FlatFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlatFlow.Infrastructure.Persistence.Configurations
{
    public class PaymentShareConfiguration : IEntityTypeConfiguration<PaymentShare>
    {
        public void Configure(EntityTypeBuilder<PaymentShare> builder)
        {
            builder.HasKey(ps => ps.Id);

            builder.Property(ps => ps.ShareAmount)
                .HasPrecision(18, 2);

            builder.Property(ps => ps.Status)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(20);

            builder.HasOne(ps => ps.Payment)
                .WithMany(p => p.PaymentShares)
                .HasForeignKey(ps => ps.PaymentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ps => ps.Tenant)
                .WithMany(t => t.PaymentShares)
                .HasForeignKey(ps => ps.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
