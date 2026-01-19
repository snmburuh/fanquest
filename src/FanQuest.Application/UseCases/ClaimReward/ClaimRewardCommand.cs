using global::FanQuest.Application.Common;
using global::FanQuest.Application.Interfaces.Repositories;
using global::FanQuest.Application.Interfaces.Services;
using global::FanQuest.Domain.Entities;
using global::FanQuest.Domain.Enums;
using Microsoft.Extensions.Logging;
namespace FanQuest.Application.UseCases.ClaimReward
{
    public record ClaimRewardCommand(Guid UserId, Guid QuestId, string PhoneNumber);
    public class ClaimRewardHandler
    {
        private readonly IQuestRepository _questRepo;
        private readonly ILeaderboardService _leaderboardService;
        private readonly IPaymentService _paymentService;
        private readonly IPaymentRepository _paymentRepo;
        private readonly ILogger<ClaimRewardHandler> _logger;

        public ClaimRewardHandler(
            IQuestRepository questRepo,
            ILeaderboardService leaderboardService,
            IPaymentService paymentService,
            IPaymentRepository paymentRepo,
            ILogger<ClaimRewardHandler> logger)
        {
            _questRepo = questRepo;
            _leaderboardService = leaderboardService;
            _paymentService = paymentService;
            _paymentRepo = paymentRepo;
            _logger = logger;
        }

        public async Task<Result<decimal>> HandleAsync(ClaimRewardCommand command)
        {
            _logger.LogInformation("User {UserId} claiming reward for quest {QuestId}",
                command.UserId, command.QuestId);

            var quest = await _questRepo.GetByIdAsync(command.QuestId);
            if (quest == null)
                return Result<decimal>.Failure("Quest not found");

            if (quest.Status != QuestStatus.Completed)
                return Result<decimal>.Failure("Quest not completed yet");

            var userRank = await _leaderboardService.GetUserRankAsync(command.QuestId, command.UserId);
            if (!userRank.HasValue)
                return Result<decimal>.Failure("User not on leaderboard");

            var reward = quest.Challenges.SelectMany(_ => new List<Reward>()) // TODO: Load rewards separately
                .FirstOrDefault(r => r.IsEligible(userRank.Value));

            if (reward == null)
                return Result<decimal>.Failure("No reward available for this rank");

            // Create reward payment
            var payment = new Payment(command.UserId, command.QuestId, reward.Value, PaymentType.Reward);
            await _paymentRepo.CreateAsync(payment);

            try
            {
                await _paymentService.InitiateRewardPayoutAsync(command.UserId, reward.Value, command.PhoneNumber);
                payment.MarkSuccess($"REWARD_{Guid.NewGuid()}"); // Mock receipt
                await _paymentRepo.UpdateAsync(payment);

                _logger.LogInformation("Reward of {Amount} paid to user {UserId}", reward.Value, command.UserId);
                return Result<decimal>.Success(reward.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Reward payout failed for user {UserId}", command.UserId);
                payment.MarkFailed();
                await _paymentRepo.UpdateAsync(payment);
                return Result<decimal>.Failure("Reward payout failed");
            }
        }
    }
}
