using Shared.Domain.Enums;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Transactions.Application.DTOs;
using Transactions.Application.Interfaces;
using Transactions.Application.Responses;
using Transactions.Domain.Entities;
using Transactions.Domain.Interfaces;

namespace Transactions.Application
{
    public class TransactionManager : ITransactionManager
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionManager(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<TransactionResponse> RegisterTransaction(RegisterTransactionDTO registerTransactionDTO)
        {
            // Todo :  review validade custom exception
            try
            {
                var transaction = Transaction.Create(registerTransactionDTO.Date, registerTransactionDTO.Type, registerTransactionDTO.Amount, registerTransactionDTO.Description);

                var id = await _transactionRepository.Create(transaction);

                return new TransactionResponse
                {
                    Id = id,
                    Success = true
                };

            }
            catch (Exception) 
            {
                return new TransactionResponse
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INTERNAL_ERROR,
                    Message = "Internal error."
                };
            }
            



        }
    }
}
