using global::FanQuest.Application.Common;
using global::FanQuest.Application.Interfaces.Repositories;
using global::FanQuest.Application.Interfaces.Services;
using global::FanQuest.Domain.Entities;
using global::FanQuest.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace FanQuest.Application.UseCases.JoinQuest
{
    public record JoinQuestCommand(Guid UserId, Guid QuestId, string PhoneNumber);
  

    public class JoinQuestHandler
    {
        private readonly IQuestRepository _questRepo;
        private readonly IParticipationRepository _participationRepo;
        private readonly IPaymentService _paymentService;
        private readonly IPaymentRepository _paymentRepo;
        private readonly ILogger<JoinQuestHandler> _logger;

        public JoinQuestHandler(
            IQuestRepository questRepo,
            IParticipationRepository participationRepo,
            IPaymentService paymentService,
            IPaymentRepository paymentRepo,
            ILogger<JoinQuestHandler> logger)
        {
            _questRepo = questRepo;
            _participationRepo = participationRepo;
            _paymentService = paymentService;
            _paymentRepo = paymentRepo;
            _logger = logger;
        }

        public async Task<Result<Guid>> HandleAsync(JoinQuestCommand command)
        {
            _logger.LogInformation("User {UserId} joining quest {QuestId}", command.UserId, command.QuestId);

            var quest = await _questRepo.GetByIdAsync(command.QuestId);
            if (quest == null)
                return Result<Guid>.Failure("Quest not found");

            if (!quest.IsActive())
                return Result<Guid>.Failure("Quest is not active");

            var existingParticipation = await _participationRepo.GetByUserAndQuestAsync(command.UserId, command.QuestId);
            if (existingParticipation != null)
                return Result<Guid>.Failure("Already participating in this quest");

            // Handle entry fee if required
            if (quest.EntryFee > 0)
            {
                var payment = new Payment(command.UserId, command.QuestId, quest.EntryFee, PaymentType.Entry);
                await _paymentRepo.CreateAsync(payment);

                try
                {
                    var mpesaReceipt = await _paymentService.InitiateEntryFeeAsync(
                        command.UserId, command.QuestId, quest.EntryFee, command.PhoneNumber);

                    payment.MarkSuccess(mpesaReceipt);
                    await _paymentRepo.UpdateAsync(payment);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Payment failed for user {UserId}", command.UserId);
                    payment.MarkFailed();
                    await _paymentRepo.UpdateAsync(payment);
                    return Result<Guid>.Failure("Payment processing failed");
                }
            }

            var participation = new Participation(command.UserId, command.QuestId);
            await _participationRepo.CreateAsync(participation);

            _logger.LogInformation("User {UserId} successfully joined quest {QuestId}", command.UserId, command.QuestId);
            return Result<Guid>.Success(participation.Id);
        }
    }
}
