using System;
using System.Collections.Generic;
using System.Text;
using Transactions.Application.DTOs;
using Transactions.Application.Responses;

namespace Transactions.Application.Interfaces
{
    public interface ITransactionManager
    {

        public Task<TransactionResponse> RegisterTransaction(RegisterTransactionDTO registerTransactionDTO);
    }
}
