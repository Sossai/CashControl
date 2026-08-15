using Shared.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Domain.Entities
{
    public record ProcessTransaction(
        Guid EventId,
        DateOnly Date,
        TransactionType Type,
        decimal Amount,
        DateTime? PublishedAt);
}
