using Consolidation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consolidation.Infrastructure
{
    public class ConsolidatesDbContext : DbContext
    {
        public ConsolidatesDbContext(DbContextOptions<ConsolidatesDbContext> options) : base(options) { }

        public DbSet<DailyConsolidate> DailyConsolidate { get; set; }
        public DbSet<ProcessedEvent> ProcessedEvent { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DailyConsolidate>()
                .HasKey(x => x.Date);

            modelBuilder.Entity<ProcessedEvent>()
                .HasKey(x => x.EventId);
        }
    }
}
