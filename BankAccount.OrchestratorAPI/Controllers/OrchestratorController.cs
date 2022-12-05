using BankAccount.Shared.OrchestorService;
using BankAccount.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;
using static BankAccount.Shared.Domain.RecordTypes;
using static BankAccount.Shared.Utilities.Enumeration;

namespace BankAccount.OrchestratorAPI.Controllers
{
    [Route("[controller]")]
    public class OrchestratorController : ControllerBase
    {
        private readonly IOrchestratorService _orchestratorService;
        private readonly ILogger<OrchestratorController> _logger;

        public OrchestratorController(
            IOrchestratorService orchestratorService,
            ILogger<OrchestratorController> logger)
        {
            _orchestratorService = orchestratorService ?? throw new ArgumentNullException(nameof(orchestratorService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost(nameof(InitiateWorkFlow))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<OperationResult<string>>> InitiateWorkFlow([FromBody] InitiateWorkFlowPayload payload)
        {
            try
            {
                var result = await _orchestratorService.InitiateWorkFlow(payload);
                return StatusCode((int)result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(statusCode: 500, Constants.InternalServerErrorMessage);
            }
        }

        [HttpGet(nameof(WorkFlows))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<EnumModel>> WorkFlows()
        {
            try
            {
                return Ok(WorkFlow.PotentialMember.ToEnumModelList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(statusCode: 500, Constants.InternalServerErrorMessage);
            }
        }
    }
}
