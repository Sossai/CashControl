using Consolidation.Domain.Interfaces;
using Consolidation.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consolidation.Infrastructure
{
    public class ConsolidateUnitOfWork : IConsolidateUnitOfWork
    {

        private readonly ConsolidatesDbContext _consolidatesDbContext;

        public ConsolidateUnitOfWork(ConsolidatesDbContext consolidatesDbContext, IDailyConsolidateRepository dailyConsolidateRepository, IProcessedEventRepository processedEventRepository)
        {
            _consolidatesDbContext = consolidatesDbContext;
            DailyConsolidateRepository = dailyConsolidateRepository;
            ProcessedEventRepository = processedEventRepository;
        }

        public IDailyConsolidateRepository DailyConsolidateRepository { get; }

        public IProcessedEventRepository ProcessedEventRepository { get; }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _consolidatesDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
