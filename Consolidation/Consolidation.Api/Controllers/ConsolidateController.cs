using Consolidation.Application;
using Consolidation.Application.Interfaces;
using Consolidation.Application.Responses;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;

namespace Consolidation.Api.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class ConsolidateController : Controller
    {
        private readonly IConsolidateManager _consolidateManager;
        public ConsolidateController(IConsolidateManager consolidateManager)
        {
            _consolidateManager = consolidateManager;
        }

        [HttpGet]
        public async Task<ActionResult<ConsolidationResponse>> GetConsolidate(DateOnly dateOnly)
        {
            try
            {
                var response = await _consolidateManager.GetConsolidate(dateOnly);

                if (response.ErrorCode == ErrorCodes.DATA_NOT_FOUND)
                    return NotFound(response);

                if (response.Success == false)
                    return BadRequest(response);

                return Ok(response);
            }
            catch(Exception)
            {
                return StatusCode(500, new ConsolidationResponse
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INTERNAL_ERROR,
                    Message = "Internal error."
                });
            }
        }
    }
}
