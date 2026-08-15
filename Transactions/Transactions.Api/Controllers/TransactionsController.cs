using Microsoft.AspNetCore.Mvc;
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
        public TransactionsController(ITransactionManager transactionManager)
        {
            _transactionManager = transactionManager;
        }

        // Todo : add dto validation
        [HttpPost]
        public async Task<ActionResult<TransactionResponse>> CreateTransaction(RegisterTransactionDTO transactionDto)
        {
            var response = await _transactionManager.RegisterTransaction(transactionDto);

            if(response.Success)
                return Created("", response);

            return BadRequest(response);
        }

    }
}
