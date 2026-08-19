using MassTransit;
using Microsoft.EntityFrameworkCore;
using Transactions.Domain.Entities;
using Transactions.Infrastructure.Entities;

namespace Transactions.Infrastructure
{
    public class TransactionsDbContext : DbContext
    {
        public TransactionsDbContext(DbContextOptions<TransactionsDbContext> options) : base(options) { }

        public DbSet<Transaction> Transactions { get; set; }
        //public DbSet<OutboxMessage> OutboxMessages { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddTransactionalOutboxEntities();

            base.OnModelCreating(modelBuilder);
        }
    }
}
