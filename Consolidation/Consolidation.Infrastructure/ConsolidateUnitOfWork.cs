using Consolidation.Domain.Interfaces;
using Consolidation.Infrastructure.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Consolidation.Infrastructure
{
    public class ConsolidateUnitOfWork : IConsolidateUnitOfWork
    {

        private readonly ConsolidatesDbContext _consolidatesDbContext;

        public ConsolidateUnitOfWork(ConsolidatesDbContext consolidatesDbContext)
        {
            _consolidatesDbContext = consolidatesDbContext;
        }

        public async Task ExecuteInTransactionAsync(Func<Task> action)
        {
            var strategy = _consolidatesDbContext.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction =
                    await _consolidatesDbContext.Database.BeginTransactionAsync();

                try
                {
                    await action();

                    await _consolidatesDbContext.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();

                    throw;
                }
            });
        }
    }
}
