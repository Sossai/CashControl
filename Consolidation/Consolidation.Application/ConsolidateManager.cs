using Consolidation.Application.Interfaces;
using Consolidation.Application.Responses;
using Consolidation.Domain.Interfaces;
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

        public ConsolidateManager(IDailyConsolidateRepository dailyConsolidateRepository)
        {
            _dailyConsolidateRepository = dailyConsolidateRepository;
        }

        public async Task ConsolidateTransaction(ProcessTransaction processTransaction)
        {
            var processAmount = ResolveAmount(processTransaction);

            await _dailyConsolidateRepository.Process(processTransaction.Date, processAmount);
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
