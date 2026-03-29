using FlatFlow.Domain.Common;
using FlatFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FlatFlow.Infrastructure.Persistence
{
    public class FlatFlowDbContext : DbContext
    {
        public FlatFlowDbContext(DbContextOptions<FlatFlowDbContext> options) : base(options) { }
        public DbSet<Flat> Flats { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Chore> Chores { get; set; }
        public DbSet<ChoreAssignment> ChoreAssignments { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentShare> PaymentShares { get; set; }
        public DbSet<Note> Notes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>().Where(e => e.State == EntityState.Modified))
            {
                entry.Property(nameof(BaseEntity.UpdatedAt)).CurrentValue = DateTime.UtcNow;
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
