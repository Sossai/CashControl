using MassTransit;
using Shared.Domain.Entities;
using Shared.Enums;
using Transactions.Application.DTOs;
using Transactions.Application.Interfaces;
using Transactions.Application.Responses;
using Transactions.Domain.Entities;
using Transactions.Domain.Interfaces;
using Transactions.Infrastructure.Interfaces;

namespace Transactions.Application
{
    public class TransactionManager : ITransactionManager
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ITransactionUnitOfWork _transactionUnitOfWork;

        public TransactionManager(
            ITransactionRepository transactionRepository, 
            IPublishEndpoint publishEndpoint
            , ITransactionUnitOfWork transactionUnitOfWork
            )
        {
            _transactionRepository = transactionRepository;
            _publishEndpoint = publishEndpoint;
            _transactionUnitOfWork = transactionUnitOfWork;
        }

        public async Task<TransactionResponse> RegisterTransaction(RegisterTransactionDTO registerTransactionDTO)
        {
            try
            {
                var transaction = Transaction.Create(registerTransactionDTO.Date, registerTransactionDTO.Type, registerTransactionDTO.Amount, registerTransactionDTO.Description);

                await _transactionRepository.AddAsync(transaction);

                await PublishTransactionAsync(transaction);

                await _transactionUnitOfWork.SaveChangesAsync();

                return new TransactionResponse
                {
                    Id = transaction.Id,
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

        private async Task PublishTransactionAsync(Transaction transaction)
        {
            var message = new ProcessTransaction(Guid.NewGuid(), transaction.Date, transaction.Type, transaction.Amount, DateTime.UtcNow);

            await _publishEndpoint.Publish(message);
        }
    }
}
