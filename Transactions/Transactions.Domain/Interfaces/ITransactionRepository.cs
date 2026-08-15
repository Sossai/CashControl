using System;
using System.Collections.Generic;
using System.Text;
using Transactions.Domain.Entities;

namespace Transactions.Domain.Interfaces
{
    public interface ITransactionRepository
    {
        Task<Guid> Create(Transaction transaction);
    }
}
