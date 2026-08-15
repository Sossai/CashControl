using Shared.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Transactions.Application.DTOs
{
    public class RegisterTransactionDTO
    {
        public DateOnly Date { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = default!;
    }
}
