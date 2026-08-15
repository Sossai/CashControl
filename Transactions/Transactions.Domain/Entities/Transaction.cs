using Shared.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Transactions.Domain.Entities
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public DateOnly Date { get; private set; }
        public TransactionType Type { get; private set; }
        public decimal Amount { get; private set; }
        public string Description { get; private set; } = default!;
        public DateTime CreatedAt { get; private set; }

        public static Transaction Create(DateOnly date, TransactionType type, decimal amount, string description)
        {
            // Todo : Validate data

            return new Transaction
            {
                Id = Guid.NewGuid(),
                Date = date,
                Type = type,
                Amount = amount,
                Description = description,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
