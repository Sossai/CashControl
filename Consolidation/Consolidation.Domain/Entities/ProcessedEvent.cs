using System;
using System.Collections.Generic;
using System.Text;

namespace Consolidation.Domain.Entities
{
    public class ProcessedEvent
    {
        public Guid EventId { get; init; }
        public DateTime ProcessedAt { get; init; }
    }
}
