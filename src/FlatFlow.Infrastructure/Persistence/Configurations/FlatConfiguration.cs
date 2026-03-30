using FlatFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlatFlow.Infrastructure.Persistence.Configurations
{
    public class FlatConfiguration : IEntityTypeConfiguration<Flat>
    {
        public void Configure(EntityTypeBuilder<Flat> builder)
        {
            builder.HasKey(f => f.Id);

            builder.Property(f => f.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(f => f.AccessCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(f => f.AccessCode)
                .IsUnique();

            builder.OwnsOne(f => f.Address, address =>
            {
                address.Property(a => a.Street)
                    .IsRequired()
                    .HasMaxLength(200);

                address.Property(a => a.City)
                    .IsRequired()
                    .HasMaxLength(100);

                address.Property(a => a.ZipCode)
                    .IsRequired()
                    .HasMaxLength(20);

                address.Property(a => a.Country)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            builder.HasMany(f => f.Tenants)
                .WithOne(t => t.Flat)
                .HasForeignKey(t => t.FlatId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(f => f.Chores)
                .WithOne(c => c.Flat)
                .HasForeignKey(c => c.FlatId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(f => f.Payments)
                .WithOne(p => p.Flat)
                .HasForeignKey(p => p.FlatId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(f => f.Notes)
                .WithOne(n => n.Flat)
                .HasForeignKey(n => n.FlatId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(f => f.Tenants).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(f => f.Chores).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(f => f.Payments).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(f => f.Notes).UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
