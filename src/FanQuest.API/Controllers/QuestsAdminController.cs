using FanQuest.API.Contract;
using FanQuest.Application.Interfaces.Repositories;
using FanQuest.Application.UseCases.JoinQuest;
using FanQuest.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FanQuest.API.Controllers
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize] // Add [Authorize(Roles = "Admin")] for production
    public class QuestsAdminController : ControllerBase
    {
        private readonly IQuestRepository _questRepo;
        private readonly IRewardRepository _rewardRepo;
        private readonly IChallengeRepository _challengeRepo;
        private readonly ILogger<QuestsAdminController> _logger;

        public QuestsAdminController(
            IQuestRepository questRepo,
            IRewardRepository rewardRepo, IChallengeRepository challengeRepo,
            ILogger<QuestsAdminController> logger)
        {
            _questRepo = questRepo;
            _challengeRepo = challengeRepo;
            _rewardRepo = rewardRepo;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuest([FromBody] CreateQuestRequest request)
        {
            _logger.LogInformation("Creating quest: {Name}", request.Name);

            if (request.StartsAt >= request.EndsAt)
                return BadRequest(new { error = "Start time must be before end time" });

            var quest = new Quest(
                request.Name,
                request.City,
                request.StartsAt,
                request.EndsAt,
                request.EntryFee);

            if (request.PublishImmediately)
                quest.Publish();

            await _questRepo.CreateAsync(quest);

            _logger.LogInformation("Quest created: {QuestId}", quest.Id);

            return Ok(new { questId = quest.Id, status = quest.Status.ToString() });
        }

        [HttpPost("{questId}/challenges")]
        public async Task<IActionResult> AddChallenge(Guid questId, [FromBody] CreateChallengeRequest request)
        {
            _logger.LogInformation("Adding challenge to quest {QuestId}", questId);

            var quest = await _questRepo.GetByIdAsync(questId);
            if (quest == null)
                return NotFound(new { error = "Quest not found" });

            Challenge challenge = request.Type switch
            {
                "CheckIn" => new CheckInChallenge(
                    questId,
                    request.Title,
                    request.Points,
                    request.OpensAt,
                    request.ClosesAt,
                    request.LocationName ?? "Unknown Location"),

                "Timed" => new TimedChallenge(
                    questId,
                    request.Title,
                    request.Points,
                    request.OpensAt,
                    request.ClosesAt),

                "Reaction" => new ReactionChallenge(
                    questId,
                    request.Title,
                    request.Points,
                    request.OpensAt,
                    request.ClosesAt),

                _ => throw new ArgumentException($"Invalid challenge type: {request.Type}")
            };

            // Don't call quest.AddChallenge() - save directly to DbContext
            await _challengeRepo.CreateAsync(challenge); // ← CHANGED

            _logger.LogInformation("Challenge added: {ChallengeId}", challenge.Id);

            return Ok(new { challengeId = challenge.Id });
        }

        [HttpPost("{questId}/rewards")]
        public async Task<IActionResult> AddReward(Guid questId, [FromBody] CreateRewardRequest request)
        {
            _logger.LogInformation("Adding reward to quest {QuestId}", questId);

            var quest = await _questRepo.GetByIdAsync(questId);
            if (quest == null)
                return NotFound(new { error = "Quest not found" });

            var reward = new Reward(
                questId,
                request.Name,
                request.Value,
                request.MinRank,
                request.MaxRank);

            await _rewardRepo.CreateAsync(reward);

            _logger.LogInformation("Reward added: {RewardId}", reward.Id);

            return Ok(new { rewardId = reward.Id });
        }

        [HttpPut("{questId}/publish")]
        public async Task<IActionResult> PublishQuest(Guid questId)
        {
            _logger.LogInformation("Publishing quest {QuestId}", questId);

            var quest = await _questRepo.GetByIdAsync(questId);
            if (quest == null)
                return NotFound(new { error = "Quest not found" });

            quest.Publish();
            await _questRepo.UpdateAsync(quest);

            _logger.LogInformation("Quest published: {QuestId}", questId);

            return Ok(new { status = "Published" });
        }

        [HttpPut("{questId}/complete")]
        public async Task<IActionResult> CompleteQuest(Guid questId)
        {
            _logger.LogInformation("Completing quest {QuestId}", questId);

            var quest = await _questRepo.GetByIdAsync(questId);
            if (quest == null)
                return NotFound(new { error = "Quest not found" });

            quest.Complete();
            await _questRepo.UpdateAsync(quest);

            _logger.LogInformation("Quest completed: {QuestId}", questId);

            return Ok(new { status = "Completed" });
        }
    }
}
