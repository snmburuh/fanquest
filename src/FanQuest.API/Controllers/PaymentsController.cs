using FanQuest.API.Contract;
using FanQuest.API.Extensions;
using FanQuest.Application.UseCases.ClaimReward;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FanQuest.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly ClaimRewardHandler _claimRewardHandler;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(
            ClaimRewardHandler claimRewardHandler,
            ILogger<PaymentsController> logger)
        {
            _claimRewardHandler = claimRewardHandler;
            _logger = logger;
        }

        [HttpPost("claim-reward")]
        public async Task<IActionResult> ClaimReward([FromBody] ClaimRewardRequest request)
        {
            var userId = GetUserId();

            var command = new ClaimRewardCommand(userId, request.QuestId, request.PhoneNumber);
            var result = await _claimRewardHandler.HandleAsync(command);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(new { amount = result.Data });
        }

        [HttpPost("mpesa-callback")]
        public async Task<IActionResult> MpesaCallback([FromBody] MpesaCallbackDto callback)
        {
            _logger.LogInformation("M-Pesa callback received: {@Callback}", callback);

            // TODO: Process callback, update payment status

            return Ok();
        }

        private Guid GetUserId()
        {
            return User.GetUserId();
        }
    }
}
