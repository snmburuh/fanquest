using FanQuest.API.Contract.Mpesa;
using FanQuest.API.Events;
using FanQuest.Application.Interfaces.Repositories;
using FanQuest.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FanQuest.API.Controllers
{
    [ApiController]
    [Route("api/mpesa/callback")]
    public class MpesaCallbackController : ControllerBase
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<MpesaCallbackController> _logger;
        private readonly IMediator _mediator;

        public MpesaCallbackController(
            IPaymentRepository paymentRepository,
            ILogger<MpesaCallbackController> logger,
            IMediator mediator)
        {
            _paymentRepository = paymentRepository;
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost("stk")]
        public async Task<IActionResult> StkCallback([FromBody] StkCallbackPayload payload)
        {
            try
            {
                _logger.LogInformation("STK callback received: {Payload}",
                    JsonSerializer.Serialize(payload));

                var callback = payload.Body.StkCallback;

                // Find payment by CheckoutRequestID
                var payment = await _paymentRepository.GetByCheckoutRequestIdAsync(callback.CheckoutRequestID);

                if (payment == null)
                {
                    _logger.LogWarning("Payment not found for CheckoutRequestID {CheckoutRequestID}",
                        callback.CheckoutRequestID);
                    return Ok(); // Acknowledge to M-Pesa
                }

                // Process based on result code
                if (callback.ResultCode == 0)
                {
                    // Success
                    var metadata = callback.CallbackMetadata?.Item;
                    var mpesaReceipt = metadata?.FirstOrDefault(x => x.Name == "MpesaReceiptNumber")?.Value?.ToString();
                    var transactionDate = metadata?.FirstOrDefault(x => x.Name == "TransactionDate")?.Value?.ToString();
                    var phoneNumber = metadata?.FirstOrDefault(x => x.Name == "PhoneNumber")?.Value?.ToString();

                    // Update payment record
                    payment.MpesaReceipt = mpesaReceipt;
                    payment.UpdateStatus(PaymentStatus.Completed);
                    payment.UpdateCompletedAt(DateTime.UtcNow);

                    await _paymentRepository.UpdateAsync(payment);

                    _logger.LogInformation(
                        "Payment {PaymentId} completed successfully. Receipt: {Receipt}",
                        payment.Id,
                        mpesaReceipt);

                    // Trigger domain event based on payment type
                    if (payment.Type == PaymentType.Entry)
                    {
                        // Create participation after successful entry fee payment
                        await _mediator.Publish(new EntryPaymentCompletedEvent
                        {
                            UserId = payment.UserId,
                            QuestId = payment.QuestId,
                            PaymentId = payment.Id,
                            Amount = payment.Amount
                        });
                    }
                }
                else
                {
                    // Payment failed
                    payment.UpdateStatus(PaymentStatus.Failed);
                    payment.FailureReason = callback.ResultDesc;

                    await _paymentRepository.UpdateAsync(payment);

                    _logger.LogWarning(
                        "Payment {PaymentId} failed. Code: {ResultCode}, Reason: {Reason}",
                        payment.Id,
                        callback.ResultCode,
                        callback.ResultDesc);
                }

                return Ok(new { ResultCode = 0, ResultDesc = "Accepted" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing STK callback");
                // Still return OK to M-Pesa to acknowledge receipt
                return Ok(new { ResultCode = 0, ResultDesc = "Accepted" });
            }
        }

        [HttpPost("b2c")]
        public async Task<IActionResult> B2cCallback([FromBody] B2cCallbackPayload payload)
        {
            try
            {
                _logger.LogInformation("B2C callback received: {Payload}",
                    JsonSerializer.Serialize(payload));

                var result = payload.Result;

                // Find payment by ConversationID or OriginatorConversationID
                var payment = await _paymentRepository.GetByConversationIdAsync(
                    result.ConversationID);

                if (payment == null)
                {
                    _logger.LogWarning(
                        "Payment not found for ConversationID {ConversationID}",
                        result.ConversationID);
                    return Ok();
                }

                if (result.ResultCode == 0)
                {
                    // B2C success
                    var transactionId = result.TransactionID;

                    payment.MpesaReceipt = transactionId;
                    payment.UpdateStatus(PaymentStatus.Completed);
                    payment.UpdateCompletedAt(DateTime.UtcNow);

                    await _paymentRepository.UpdateAsync(payment);

                    _logger.LogInformation(
                        "Reward payment {PaymentId} completed. TransactionID: {TransactionId}",
                        payment.Id,
                        transactionId);
                }
                else
                {
                    // B2C failed
                    payment.UpdateStatus(PaymentStatus.Failed);
                    payment.FailureReason = result.ResultDesc;

                    await _paymentRepository.UpdateAsync(payment);

                    _logger.LogWarning(
                        "Reward payment {PaymentId} failed. Code: {ResultCode}, Reason: {Reason}",
                        payment.Id,
                        result.ResultCode,
                        result.ResultDesc);
                }

                return Ok(new { ResultCode = 0, ResultDesc = "Accepted" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing B2C callback");
                return Ok(new { ResultCode = 0, ResultDesc = "Accepted" });
            }
        }
    }
}
