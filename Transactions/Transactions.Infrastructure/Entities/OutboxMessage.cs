using Shared.Domain.Enums;
using System.Text.Json;
using Transactions.Infrastructure.Enums;

namespace Transactions.Infrastructure.Entities
{
    public class OutboxMessage
    {
        public Guid Id { get; init; }
        public Guid EventId { get; init; }
        public string Payload { get; init; }
        public OutboxStatus Status { get; private set; }
        public DateTime CreatedAt { get; init; }
        public DateTime? PublishedAt { get; private set; }

        public static OutboxMessage Create(Guid eventId, Object payload)
        {
            return new OutboxMessage
            {
                Id = Guid.NewGuid(),
                EventId = eventId,
                Payload = JsonSerializer.Serialize(payload),
                Status = OutboxStatus.Published,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
