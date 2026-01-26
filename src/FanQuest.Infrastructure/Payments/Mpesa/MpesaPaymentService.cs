using FanQuest.Application.Interfaces.Repositories;
using FanQuest.Application.Interfaces.Services;
using FanQuest.Domain.Entities;
using FanQuest.Domain.Enums;
using FanQuest.Infrastructure.Payments.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanQuest.Infrastructure.Payments.Mpesa
{
    public class MpesaPaymentService : IPaymentService
    {
        private readonly IMpesaClient _mpesaClient;
        private readonly IMpesaConfigService _configService;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IQuestRepository _questRepository;
        private readonly ILogger<MpesaPaymentService> _logger;

        public MpesaPaymentService(
            IMpesaClient mpesaClient,
            IMpesaConfigService configService,
            IPaymentRepository paymentRepository,
            IQuestRepository questRepository,
            ILogger<MpesaPaymentService> logger)
        {
            _mpesaClient = mpesaClient;
            _configService = configService;
            _paymentRepository = paymentRepository;
            _questRepository = questRepository;
            _logger = logger;
        }

        public async Task<Payment> InitiateEntryFeeAsync(Guid userId, Guid questId, string phoneNumber)
        {
            try
            {
                var quest = await _questRepository.GetByIdAsync(questId);
                if (quest == null)
                    throw new ArgumentException("Quest not found");

                // Get tenant-specific M-Pesa config
                var config = await _configService.GetConfigurationAsync(quest.TenantId);

                // Create payment record
                var payment = new Payment(userId, questId, quest.TenantId, quest.EntryFee, PaymentType.Entry, phoneNumber);
                await _paymentRepository.AddAsync(payment);

                // Generate STK Push password
                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                var password = Convert.ToBase64String(
                    Encoding.UTF8.GetBytes($"{config.BusinessShortCode}{config.Passkey}{timestamp}")
                );

                // Prepare STK Push request
                var stkRequest = new StkPushRequest
                {
                    BusinessShortCode = config.BusinessShortCode,
                    Password = password,
                    Timestamp = timestamp,
                    Amount = quest.EntryFee.ToString("F0"),
                    PartyA = phoneNumber,
                    PartyB = config.BusinessShortCode,
                    PhoneNumber = phoneNumber,
                    CallBackURL = config.StkCallbackUrl,
                    AccountReference = $"Quest-{questId}",
                    TransactionDesc = $"Entry fee for {quest.Name}"
                };

                // Initiate STK Push
                var response = await _mpesaClient.InitiateStkPushAsync(stkRequest, config.TenantId);

                if (response.ResponseCode != "0")
                {
                    payment.MarkAsFailed(response.ResponseDescription);
                    await _paymentRepository.UpdateAsync(payment);
                    throw new MpesaException($"STK Push failed: {response.ResponseDescription}");
                }

                // Update payment with M-Pesa references
                payment.SetMpesaRequest(response.MerchantRequestID, response.CheckoutRequestID);
                payment.UpdateStatus(PaymentStatus.Processing);
                await _paymentRepository.UpdateAsync(payment);

                _logger.LogInformation(
                    "STK Push initiated for user {UserId}, quest {QuestId}, CheckoutRequestID {CheckoutRequestID}",
                    userId, questId, response.CheckoutRequestID);

                return payment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initiating entry fee payment for user {UserId}, quest {QuestId}",
                    userId, questId);
                throw;
            }
        }

        public async Task<bool> VerifyPaymentAsync(Guid paymentId)
        {
            try
            {
                var payment = await _paymentRepository.GetByIdAsync(paymentId);
                if (payment == null)
                {
                    _logger.LogWarning("Payment {PaymentId} not found", paymentId);
                    return false;
                }

                // Check if already completed or failed
                if (payment.Status == PaymentStatus.Completed)
                    return true;

                if (payment.Status == PaymentStatus.Failed || payment.Status == PaymentStatus.Cancelled)
                    return false;

                // Check callback timeout
                var config = await _configService.GetConfigurationAsync(payment.TenantId);
                var timeSinceCreation = DateTime.UtcNow - payment.CreatedAt;

                if (timeSinceCreation.TotalMinutes > config.CallbackTimeoutMinutes && payment.LastStatusCheckAt == null)
                {
                    _logger.LogInformation("Payment {PaymentId} callback timeout, querying status", paymentId);
                    await QueryPaymentStatusAsync(payment, config);
                }

                // Reload payment after potential update
                payment = await _paymentRepository.GetByIdAsync(paymentId);
                return payment.Status == PaymentStatus.Completed;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying payment {PaymentId}", paymentId);
                return false;
            }
        }

        public async Task<Payment> InitiateRewardPayoutAsync(Guid userId, Guid questId, decimal amount, string phoneNumber)
        {
            try
            {
                var quest = await _questRepository.GetByIdAsync(questId);
                if (quest == null)
                    throw new ArgumentException("Quest not found");

                // Get tenant-specific M-Pesa config
                var config = await _configService.GetConfigurationAsync(quest.TenantId);

                // Create payment record
                var payment = new Payment(userId, questId, quest.TenantId, amount, PaymentType.Reward, phoneNumber);
                await _paymentRepository.AddAsync(payment);

                // Generate unique conversation ID
                var originatorConversationId = $"FQ_{Guid.NewGuid():N}_{DateTime.UtcNow:yyyyMMddHHmmss}";

                // Prepare B2C request
                var b2cRequest = new B2CRequest
                {
                    OriginatorConversationID = originatorConversationId,
                    InitiatorName = config.InitiatorName,
                    SecurityCredential = config.SecurityCredential,
                    Amount = amount.ToString("F0"),
                    PartyA = config.BusinessShortCode,
                    PartyB = phoneNumber,
                    Remarks = $"Reward payout for {quest.Name}",
                    QueueTimeOutURL = config.B2CQueueTimeoutUrl,
                    ResultURL = config.B2CResultUrl,
                    Occassion = $"Quest-{questId}"
                };

                // Initiate B2C
                var response = await _mpesaClient.InitiateB2CAsync(b2cRequest, config.TenantId);

                if (response.ResponseCode != "0")
                {
                    payment.MarkAsFailed(response.ResponseDescription);
                    await _paymentRepository.UpdateAsync(payment);
                    throw new MpesaException($"B2C failed: {response.ResponseDescription}");
                }

                // Update payment with M-Pesa references
                payment.SetB2CRequest(response.ConversationID, response.OriginatorConversationID);
                payment.UpdateStatus(PaymentStatus.Processing);
                await _paymentRepository.UpdateAsync(payment);

                _logger.LogInformation(
                    "B2C initiated for user {UserId}, quest {QuestId}, ConversationID {ConversationID}",
                    userId, questId, response.ConversationID);

                return payment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initiating reward payout for user {UserId}, quest {QuestId}",
                    userId, questId);
                throw;
            }
        }

        private async Task QueryPaymentStatusAsync(Payment payment, MpesaConfiguration config)
        {
            try
            {
                payment.IncrementStatusCheck();
                await _paymentRepository.UpdateAsync(payment);

                if (payment.StatusCheckAttempts > 3)
                {
                    _logger.LogWarning("Payment {PaymentId} exceeded max status check attempts", payment.Id);
                    payment.UpdateStatus(PaymentStatus.TimedOut);
                    await _paymentRepository.UpdateAsync(payment);
                    return;
                }

                var statusRequest = new TransactionStatusRequest
                {
                    Initiator = config.InitiatorName,
                    SecurityCredential = config.SecurityCredential,
                    TransactionID = payment.CheckoutRequestId ?? payment.ConversationId,
                    OriginalConversationID = payment.MerchantRequestId ?? payment.OriginatorConversationId,
                    PartyA = config.BusinessShortCode,
                    IdentifierType = "4",
                    ResultURL = config.StkCallbackUrl,
                    QueueTimeOutURL = config.B2CQueueTimeoutUrl,
                    Remarks = "Status check",
                    Occasion = $"Payment-{payment.Id}"
                };

                await _mpesaClient.QueryTransactionStatusAsync(statusRequest.TransactionID, config.TenantId);

                _logger.LogInformation("Transaction status query initiated for payment {PaymentId}", payment.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying payment status for payment {PaymentId}", payment.Id);
            }
        }

        public Task<Payment> InitiateEntryFeeAsync(Guid userId, Guid questId, decimal entryFee, string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public Task InitiateRewardPayoutAsync(Guid userId, decimal value, string phoneNumber)
        {
            throw new NotImplementedException();
        }
    }
}
