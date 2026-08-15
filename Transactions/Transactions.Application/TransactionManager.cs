using MassTransit;
using MassTransit.Transports;
using Shared.Domain.Entities;
using Shared.Domain.Enums;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
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
        private readonly IPublishEndpoint _publishEndpoint;
        //private readonly ISendEndpoint _sendEndpoint;

        public TransactionManager(
            ITransactionRepository transactionRepository, 
            IPublishEndpoint publishEndpoint
            //ISendEndpoint sendEndpoint
            )
        {
            _transactionRepository = transactionRepository;
            _publishEndpoint = publishEndpoint;
            //_sendEndpoint = sendEndpoint;

        }

        public async Task<TransactionResponse> RegisterTransaction(RegisterTransactionDTO registerTransactionDTO)
        {
            // Todo :  review validade custom exception
            try
            {
                var transaction = Transaction.Create(registerTransactionDTO.Date, registerTransactionDTO.Type, registerTransactionDTO.Amount, registerTransactionDTO.Description);

                //var id = await _transactionRepository.Create(transaction);
                transaction.Id = await _transactionRepository.Create(transaction);


                //todo review 
                await PublishTransactionAsync(transaction);

                // Todo: review use full object
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
            var id = Guid.NewGuid();
            //var payload = JsonSerializer.Serialize(transaction);

            //var message = new OutboxMessage(id, EventType.Transaction, payload, DateTime.UtcNow);
            var message = new ProcessTransaction(id, transaction.Date, transaction.Type, transaction.Amount, DateTime.UtcNow);

            await _publishEndpoint.Publish(message);
            
        }
    }
}
