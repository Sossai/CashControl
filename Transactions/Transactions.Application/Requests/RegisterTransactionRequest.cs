using Shared.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Transactions.Application.Requests
{
    public class RegisterTransactionRequest
    {
        public string Date { get; set; }
        public short Type { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = default!;
    }
}
