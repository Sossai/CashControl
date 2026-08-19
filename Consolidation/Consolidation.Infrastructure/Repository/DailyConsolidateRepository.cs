using Consolidation.Domain.Entities;
using Consolidation.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Consolidation.Infrastructure.Repository
{
    public class DailyConsolidateRepository : IDailyConsolidateRepository
    {
        private readonly ConsolidatesDbContext _consolidatesDbContext;

        public DailyConsolidateRepository(ConsolidatesDbContext consolidatesDbContext)
        {
            _consolidatesDbContext = consolidatesDbContext;
        }

        public async Task<DailyConsolidate?> GetConsolidate(DateOnly date)
        {
            return await _consolidatesDbContext.DailyConsolidate
                .Where(d => d.Date == date)
                .FirstOrDefaultAsync();
        }

        public async Task Process(DateOnly date, decimal amount)
        {
            var now = DateTime.UtcNow;

            //to prevent competition
            await _consolidatesDbContext.DailyConsolidate
                .Upsert(new DailyConsolidate
                {
                    Date = date,
                    AccumulatedBalance = amount,
                    UpdatedAt = now
                })
                .On(x => x.Date)
                .WhenMatched(x => new DailyConsolidate
                {
                    AccumulatedBalance = x.AccumulatedBalance + amount,
                    UpdatedAt = now
                })
                .RunAsync();
        }
    }
}
