using Shared.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Transactions.Application.Responses
{
    public class TransactionResponse : Response
    {
        public Guid? Id { get; set; }
    }
}
