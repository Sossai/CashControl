using Consolidation.Application.Responses;
using Shared.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consolidation.Application.Interfaces
{
    public interface IConsolidateManager
    {
        Task ConsolidateTransaction(ProcessTransaction processTransaction);
        Task<ConsolidationResponse> GetConsolidate(DateOnly date);
    }
}
