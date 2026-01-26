using FanQuest.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanQuest.Domain.Entities
{
    public class Payment
    {
        private decimal value;
        private PaymentType reward;

        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public Guid QuestId { get; private set; }
        public Guid TenantId { get; private set; }
        public decimal Amount { get; private set; }
        public PaymentType Type { get; private set; }
        public PaymentStatus Status { get; private set; }

        // M-Pesa specific fields
        public string MerchantRequestId { get; private set; }
        public string CheckoutRequestId { get; private set; }
        public string ConversationId { get; private set; }
        public string OriginatorConversationId { get; private set; }
        public string MpesaReceiptNumber { get; private set; }
        public string PhoneNumber { get; private set; }
        public DateTime? TransactionDate { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public DateTime? LastStatusCheckAt { get; private set; }
        public int StatusCheckAttempts { get; private set; }
        public string ErrorMessage { get; private set; }
        public string? MpesaReceipt { get; set; }
        public string FailureReason { get; set; }

        protected Payment() { }

        public void UpdateStatus(PaymentStatus newStatus)
        {
            Status = newStatus;
        }

        public void UpdateCompletedAt(DateTime? CompletedAt)
        {
            CompletedAt = CompletedAt;
        }

        public Payment(Guid userId, Guid questId, Guid tenantId, decimal amount, PaymentType type, string phoneNumber)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            QuestId = questId;
            TenantId = tenantId;
            Amount = amount;
            Type = type;
            PhoneNumber = phoneNumber;
            Status = PaymentStatus.Pending;
            CreatedAt = DateTime.UtcNow;
            StatusCheckAttempts = 0;
        }

        public Payment(Guid userId, Guid questId, decimal value, PaymentType reward)
        {
            UserId = userId;
            QuestId = questId;
            this.value = value;
            this.reward = reward;
        }

        public void SetMpesaRequest(string merchantRequestId, string checkoutRequestId)
        {
            MerchantRequestId = merchantRequestId;
            CheckoutRequestId = checkoutRequestId;
        }

        public void SetB2CRequest(string conversationId, string originatorConversationId)
        {
            ConversationId = conversationId;
            OriginatorConversationId = originatorConversationId;
        }

        public void MarkAsCompleted(string mpesaReceipt, DateTime transactionDate)
        {
            Status = PaymentStatus.Completed;
            MpesaReceiptNumber = mpesaReceipt;
            TransactionDate = transactionDate;
            CompletedAt = DateTime.UtcNow;
        }

        public void MarkAsFailed(string errorMessage)
        {
            Status = PaymentStatus.Failed;
            ErrorMessage = errorMessage;
            CompletedAt = DateTime.UtcNow;
        }

        public void IncrementStatusCheck()
        {
            StatusCheckAttempts++;
            LastStatusCheckAt = DateTime.UtcNow;
        }

        public void MarkSuccess(string v)
        {
            throw new NotImplementedException();
        }

        public void MarkFailed()
        {
            throw new NotImplementedException();
        }

        public void MarkSuccess(Payment mpesaReceipt)
        {
            throw new NotImplementedException();
        }
    }
}
