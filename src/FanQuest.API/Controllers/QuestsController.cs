using FanQuest.API.Contract;
using FanQuest.API.Extensions;
using FanQuest.Application.DTOs;
using FanQuest.Application.Interfaces.Repositories;
using FanQuest.Application.UseCases.JoinQuest;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FanQuest.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class QuestsController : ControllerBase
    {
        private readonly IQuestRepository _questRepo;
        private readonly JoinQuestHandler _joinQuestHandler;
        private readonly ILogger<QuestsController> _logger;

        public QuestsController(
            IQuestRepository questRepo,
            JoinQuestHandler joinQuestHandler,
            ILogger<QuestsController> logger)
        {
            _questRepo = questRepo;
            _joinQuestHandler = joinQuestHandler;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetQuests([FromQuery] string? city = null)
        {
            var quests = await _questRepo.GetActiveQuestsAsync(city);

            var dtos = quests.Select(q => new QuestDto
            {
                Id = q.Id,
                Name = q.Name,
                City = q.City,
                StartsAt = q.StartsAt,
                EndsAt = q.EndsAt,
                EntryFee = q.EntryFee,
                Status = q.Status.ToString(),
                ChallengeCount = q.Challenges.Count
            });

            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuest(Guid id)
        {
            var quest = await _questRepo.GetByIdAsync(id);

            if (quest == null)
                return NotFound(new { error = "Quest not found" });

            var dto = new QuestDto
            {
                Id = quest.Id,
                Name = quest.Name,
                City = quest.City,
                StartsAt = quest.StartsAt,
                EndsAt = quest.EndsAt,
                EntryFee = quest.EntryFee,
                Status = quest.Status.ToString(),
                ChallengeCount = quest.Challenges.Count
            };

            return Ok(dto);
        }

        [HttpPost("{questId}/join")]
        public async Task<IActionResult> JoinQuest(Guid questId, [FromBody] JoinQuestRequest request)
        {
            var userId = GetUserId(); // Your method to extract user ID from token/session

            var command = new JoinQuestCommand(
                userId,
                questId,
                request.PhoneNumber);

            var result = await _joinQuestHandler.HandleAsync(command);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(new { participationId = result.Data });
        }

        private Guid GetUserId()
        {
            return User.GetUserId();
        }
    }
}
