using Consolidation.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consolidation.Infrastructure.Interfaces
{
    public interface IConsolidateUnitOfWork
    {
        IDailyConsolidateRepository DailyConsolidateRepository { get; }
        IProcessedEventRepository ProcessedEventRepository { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
