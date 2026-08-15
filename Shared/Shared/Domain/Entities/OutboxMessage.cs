using Shared.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Domain.Entities
{
    public record OutboxMessage(
        Guid EventId,
        EventType EventType,
        string Payload,
        DateTime? PublishedAt);

}
