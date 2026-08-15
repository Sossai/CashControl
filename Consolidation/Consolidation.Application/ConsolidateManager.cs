using Consolidation.Application.Interfaces;
using Consolidation.Application.Responses;
using Consolidation.Domain.Interfaces;
using Consolidation.Infrastructure.Interfaces;
using Shared.Domain.Entities;
using Shared.Domain.Enums;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consolidation.Application
{
    public class ConsolidateManager : IConsolidateManager
    {
        private readonly IDailyConsolidateRepository _dailyConsolidateRepository;
        private readonly IProcessedEventRepository _processedEventRepository;
        private readonly IConsolidateUnitOfWork _consolidateUnitOfWork;

        public ConsolidateManager(
            IDailyConsolidateRepository dailyConsolidateRepository,
            IProcessedEventRepository processedEventRepository,
            IConsolidateUnitOfWork consolidateUnitOfWork
            )
        {
            _dailyConsolidateRepository = dailyConsolidateRepository;
            _processedEventRepository = processedEventRepository;
            _consolidateUnitOfWork = consolidateUnitOfWork;
        }

        public async Task ConsolidateTransaction(ProcessTransaction processTransaction)
        {
            // idempotency validator
            if (await _processedEventRepository.IsProcessedAsync(processTransaction.EventId))
                return;


            var processAmount = ResolveAmount(processTransaction);

            await _consolidateUnitOfWork.DailyConsolidateRepository.Process(processTransaction.Date, processAmount);
            await _consolidateUnitOfWork.ProcessedEventRepository.RegisterProcessedAsync(processTransaction.EventId);

            await _consolidateUnitOfWork.SaveChangesAsync();

            //await _dailyConsolidateRepository.Process(processTransaction.Date, processAmount);
            //await _processedEventRepository.RegisterProcessedAsync(processTransaction.EventId);
        }

        public async Task<ConsolidationResponse> GetConsolidate(DateOnly date)
        {
            var dailyConsolidate = await _dailyConsolidateRepository.GetConsolidate(date);
            if (dailyConsolidate == null)
            {
                return new ConsolidationResponse
                {
                    Success = false,
                    ErrorCode = ErrorCodes.DATA_NOT_FOUND,
                    Message = "Consolidation data not found.",
                    Date = date,
                    Amount = 0
                };
            }

            return new ConsolidationResponse
            {
                Success = true,
                Date = date,
                Amount = dailyConsolidate.AccumulatedBalance,
                UpdatedAt = dailyConsolidate.UpdatedAt
            }; 
        }

        private static decimal ResolveAmount(ProcessTransaction processTransaction)
        {
            if (processTransaction.Type == TransactionType.Debit)
                return processTransaction.Amount * -1;

            return processTransaction.Amount;
        }

    }
}
