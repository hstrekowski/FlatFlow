using FlatFlow.Domain.Common;
using FlatFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

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
            var modifiedEntries = ChangeTracker.Entries<BaseEntity>().Where(e => e.State == EntityState.Modified);

            var methodInfo = typeof(BaseEntity).GetMethod("SetUpdatedAt", BindingFlags.Instance | BindingFlags.NonPublic);

            if (methodInfo != null)
            {
                foreach (var entry in modifiedEntries)
                {
                    methodInfo.Invoke(entry.Entity, null);
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
