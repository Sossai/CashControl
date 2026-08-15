using Consolidation.Application;
using Consolidation.Application.Interfaces;
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
        public async Task<ActionResult<Decimal>> GetConsolidate(DateOnly dateOnly)
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
                return Problem(detail: "Internal server error.", statusCode: 500);
            }
        }
    }
}
