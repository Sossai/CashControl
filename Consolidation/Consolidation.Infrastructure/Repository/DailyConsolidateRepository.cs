using Consolidation.Domain.Entities;
using Consolidation.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

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
            //var affectedRows = await _consolidatesDbContext.DailyConsolidate
            //    .Where(x => x.Date == date)
            //    .ExecuteUpdateAsync(setters => setters
            //        .SetProperty(
            //            x => x.AccumulatedBalance,
            //            x => x.AccumulatedBalance + amount)
            //        .SetProperty(
            //            x => x.UpdatedAt,
            //            DateTime.UtcNow));

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
