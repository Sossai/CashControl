using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using Transactions.Application.DTOs;
using Transactions.Application.Interfaces;
using Transactions.Application.Responses;

namespace Consolidation.Api.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class TransactionsController : Controller
    {
        public readonly ITransactionManager _transactionManager;
        private readonly IValidator<RegisterTransactionDTO> _validator;

        public TransactionsController(
            ITransactionManager transactionManager,
            IValidator<RegisterTransactionDTO> validator            )
        {
            _transactionManager = transactionManager;
            _validator = validator;
        }

        [HttpPost]
        public async Task<ActionResult<TransactionResponse>> CreateTransaction(RegisterTransactionDTO transactionDto)
        {
            try
            {
                var result = await _validator.ValidateAsync(transactionDto);
                if (!result.IsValid)
                {
                    return BadRequest(new TransactionResponse
                    {
                        Success = false,
                        ErrorCode = ErrorCodes.MISSING_REQUIRED_INFORMATION,
                        Message = string.Join(" | ", result.Errors.Select(x => x.ErrorMessage))
                    });
                }

                var response = await _transactionManager.RegisterTransaction(transactionDto);

                if (response.Success)
                    return Created("", response);

                return BadRequest(response);
            }
            catch (Exception)
            {
                return StatusCode(500, new TransactionResponse
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INTERNAL_ERROR,
                    Message = "Internal error."
                });
            }
        }

    }
}
