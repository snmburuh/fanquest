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
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public Guid? QuestId { get; private set; }
        public decimal Amount { get; private set; }
        public PaymentType Type { get; private set; }
        public string MpesaReceipt { get; private set; }
        public PaymentStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }

        protected Payment() { } // EF Core

        public Payment(Guid userId, Guid? questId, decimal amount, PaymentType type)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            QuestId = questId;
            Amount = amount;
            Type = type;
            Status = PaymentStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkSuccess(string mpesaReceipt)
        {
            MpesaReceipt = mpesaReceipt ?? throw new ArgumentNullException(nameof(mpesaReceipt));
            Status = PaymentStatus.Success;
            CompletedAt = DateTime.UtcNow;
        }

        public void MarkFailed()
        {
            Status = PaymentStatus.Failed;
            CompletedAt = DateTime.UtcNow;
        }
    }
}
