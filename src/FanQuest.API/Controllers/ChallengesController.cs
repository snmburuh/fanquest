using FanQuest.API.Contract;
using FanQuest.API.Extensions;
using FanQuest.Application.DTOs;
using FanQuest.Application.Interfaces.Repositories;
using FanQuest.Application.UseCases.CompleteChallenge;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FanQuest.API.Controllers
{
    [ApiController]
    [Route("api/quests/{questId}/[controller]")]
    [Authorize]
    public class ChallengesController : ControllerBase
    {
        private readonly IQuestRepository _questRepo;
        private readonly IParticipationRepository _participationRepo;
        private readonly CompleteChallengeHandler _completeChallengeHandler;
        private readonly ILogger<ChallengesController> _logger;

        public ChallengesController(
            IQuestRepository questRepo,
            IParticipationRepository participationRepo,
            CompleteChallengeHandler completeChallengeHandler,
            ILogger<ChallengesController> logger)
        {
            _questRepo = questRepo;
            _participationRepo = participationRepo;
            _completeChallengeHandler = completeChallengeHandler;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetChallenges(Guid questId)
        {
            var userId = GetUserId();
            var quest = await _questRepo.GetByIdAsync(questId);

            if (quest == null)
                return NotFound(new { error = "Quest not found" });

            var participation = await _participationRepo.GetByUserAndQuestAsync(userId, questId);

            var dtos = quest.Challenges.Select(c => new ChallengeDto
            {
                Id = c.Id,
                Title = c.Title,
                Points = c.Points,
                OpensAt = c.OpensAt,
                ClosesAt = c.ClosesAt,
                Type = c.Type.ToString(),
                IsCompleted = participation?.HasCompleted(c.Id) ?? false
            });

            return Ok(dtos);
        }

        [HttpPost("{challengeId}/complete")]
        public async Task<IActionResult> CompleteChallenge(Guid questId, Guid challengeId, [FromBody] CompleteChallengeRequest request)
        {
            var userId = GetUserId();

            var command = new CompleteChallengeCommand(
                userId,
                challengeId,
                request.Timestamp ?? DateTime.UtcNow,
                request.Latitude,
                request.Longitude,
                request.Payload);

            var result = await _completeChallengeHandler.HandleAsync(command);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(new { pointsAwarded = result.Data });
        }

        private Guid GetUserId()
        {
            return User.GetUserId();
        }
    }
}
