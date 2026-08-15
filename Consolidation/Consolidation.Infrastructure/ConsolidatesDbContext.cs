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

        public DbSet<DailyConsolidate> DailyConsolidate { get; set; }    // Todo change only to set after test

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DailyConsolidate>()
                .HasKey(x => x.Date);
        }
    }
}
