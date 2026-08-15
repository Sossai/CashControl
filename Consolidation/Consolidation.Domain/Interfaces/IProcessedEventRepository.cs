using System;
using System.Collections.Generic;
using System.Text;

namespace Consolidation.Domain.Interfaces
{
    public interface IProcessedEventRepository
    {
        Task<bool> IsProcessedAsync(Guid eventId);
        Task RegisterProcessedAsync(Guid eventId);
    }
}
