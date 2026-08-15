using Consolidation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consolidation.Domain.Interfaces
{
    public interface IDailyConsolidateRepository
    {
        Task Process(DateOnly date, decimal amount);
        Task<DailyConsolidate?> GetConsolidate(DateOnly date);
    }
}
