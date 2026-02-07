using FanQuest.API.Extensions;
using FanQuest.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FanQuest.API.Controllers
{
    [ApiController]
    [Route("api/quests/{questId}/[controller]")]
    [Authorize]
    public class LeaderboardController : ControllerBase
    {
        private readonly ILeaderboardService _leaderboardService;
        private readonly ILogger<LeaderboardController> _logger;

        public LeaderboardController(
            ILeaderboardService leaderboardService,
            ILogger<LeaderboardController> logger)
        {
            _leaderboardService = leaderboardService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetLeaderboard(Guid questId, [FromQuery] int limit = 50)
        {
            var leaderboard = await _leaderboardService.GetLeaderboardAsync(questId, limit);
            return Ok(leaderboard);
        }

        [HttpGet("my-rank")]
        public async Task<IActionResult> GetMyRank(Guid questId)
        {
            var userId = GetUserId();
            var rank = await _leaderboardService.GetUserRankAsync(questId, userId);

            if (!rank.HasValue)
                return NotFound(new { error = "User not on leaderboard" });

            return Ok(new { rank = rank.Value });
        }

        private Guid GetUserId()
        {
            return User.GetUserId();
        }
    }
}
