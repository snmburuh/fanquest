using FanQuest.API.Extensions;
using FanQuest.Application.DTOs;
using FanQuest.Application.Interfaces.Repositories;
using FanQuest.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FanQuest.API.Controllers
{
    [ApiController]
    [Route("api/quests/{questId}/[controller]")]
    [Authorize]
    public class RewardsController : ControllerBase
    {
        private readonly IRewardRepository _rewardRepo;
        private readonly ILeaderboardService _leaderboardService;
        private readonly ILogger<RewardsController> _logger;

        public RewardsController(
            IRewardRepository rewardRepo,
            ILeaderboardService leaderboardService,
            ILogger<RewardsController> logger)
        {
            _rewardRepo = rewardRepo;
            _leaderboardService = leaderboardService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetRewards(Guid questId)
        {
            var userId = GetUserId();
            var rewards = await _rewardRepo.GetRewardsByQuestIdAsync(questId);
            var userRank = await _leaderboardService.GetUserRankAsync(questId, userId);

            var dtos = rewards.Select(r => new RewardDto
            {
                Id = r.Id,
                Name = r.Name,
                Value = r.Value,
                MinRank = r.MinRank,
                MaxRank = r.MaxRank,
                IsEligible = userRank.HasValue && r.IsEligible(userRank.Value)
            });

            return Ok(dtos);
        }

        private Guid GetUserId()
        {
            return User.GetUserId();
        }
    }
}
