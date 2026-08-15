using Consolidation.Domain.Entities;
using Consolidation.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Consolidation.Infrastructure.Repository
{
    public class ProcessedEventRepository : IProcessedEventRepository
    {
        private readonly ConsolidatesDbContext _consolidatesDbContext;

        public ProcessedEventRepository(ConsolidatesDbContext consolidatesDbContext)
        {
            _consolidatesDbContext = consolidatesDbContext;
        }

        public async Task<bool> IsProcessedAsync(Guid eventId)
        {
            return await _consolidatesDbContext.ProcessedEvent
                .AnyAsync(e => e.EventId == eventId);
        }

        public async Task RegisterProcessedAsync(Guid eventId)
        {
            var processedEvent = new ProcessedEvent
            {
                EventId = eventId,
                ProcessedAt = DateTime.UtcNow
            };
            _consolidatesDbContext.ProcessedEvent.Add(processedEvent);
            await _consolidatesDbContext.SaveChangesAsync();
        }
    }
}
