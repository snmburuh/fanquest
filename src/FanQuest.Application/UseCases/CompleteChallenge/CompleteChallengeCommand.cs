using global::FanQuest.Application.Common;
using global::FanQuest.Application.Interfaces.Repositories;
using global::FanQuest.Application.Interfaces.Services;
using global::FanQuest.Domain.Entities;
using global::FanQuest.Domain.Rules;
using Microsoft.Extensions.Logging;

namespace FanQuest.Application.UseCases.CompleteChallenge
{
    public record CompleteChallengeCommand(
    Guid UserId,
    Guid ChallengeId,
    DateTime Timestamp,
    double? Latitude = null,
    double? Longitude = null,
    string? Payload = null);



    public class CompleteChallengeHandler
    {
        private readonly IQuestRepository _questRepo;
        private readonly IParticipationRepository _participationRepo;
        private readonly IUserRepository _userRepo;
        private readonly ILeaderboardService _leaderboardService;
        private readonly ChallengeRuleEngine _ruleEngine;
        private readonly ILogger<CompleteChallengeHandler> _logger;

        public CompleteChallengeHandler(
            IQuestRepository questRepo,
            IParticipationRepository participationRepo,
            IUserRepository userRepo,
            ILeaderboardService leaderboardService,
            ChallengeRuleEngine ruleEngine,
            ILogger<CompleteChallengeHandler> logger)
        {
            _questRepo = questRepo;
            _participationRepo = participationRepo;
            _userRepo = userRepo;
            _leaderboardService = leaderboardService;
            _ruleEngine = ruleEngine;
            _logger = logger;
        }

        public async Task<Result<int>> HandleAsync(CompleteChallengeCommand command)
        {
            _logger.LogInformation("User {UserId} attempting challenge {ChallengeId}",
                command.UserId, command.ChallengeId);

            // Find challenge through quest
            var quests = await _questRepo.GetActiveQuestsAsync();
            var challenge = quests.SelectMany(q => q.Challenges)
                .FirstOrDefault(c => c.Id == command.ChallengeId);

            if (challenge == null)
                return Result<int>.Failure("Challenge not found");

            var participation = await _participationRepo.GetByUserAndQuestAsync(command.UserId, challenge.QuestId);
            if (participation == null)
                return Result<int>.Failure("User not participating in this quest");

            if (participation.HasCompleted(command.ChallengeId))
                return Result<int>.Failure("Challenge already completed");

            // Evaluate rules
            var context = new ChallengeContext
            {
                UserId = command.UserId,
                QuestId = challenge.QuestId,
                ChallengeId = command.ChallengeId,
                Timestamp = command.Timestamp,
                Latitude = command.Latitude,
                Longitude = command.Longitude,
                Payload = command.Payload
            };

            var result = _ruleEngine.EvaluateAll(challenge, context);

            if (!result.IsSuccessful)
            {
                _logger.LogWarning("Challenge completion failed: {Reason}", result.Reason);
                return Result<int>.Failure(result.Reason);
            }

            // Record completion
            var completion = new Completion(participation.Id, command.ChallengeId, result.PointsAwarded);
            participation.AddCompletion(completion);
            participation.AddScore(result.PointsAwarded);
            await _participationRepo.UpdateAsync(participation);

            // Update user total points
            var user = await _userRepo.GetByIdAsync(command.UserId);
            if (user != null)
            {
                user.AddPoints(result.PointsAwarded);
                await _userRepo.UpdateAsync(user);
            }

            // Update leaderboard
            await _leaderboardService.UpdateScoreAsync(challenge.QuestId, command.UserId, participation.Score);

            _logger.LogInformation("Challenge {ChallengeId} completed, awarded {Points} points",
                command.ChallengeId, result.PointsAwarded);

            return Result<int>.Success(result.PointsAwarded);
        }
    }
}
