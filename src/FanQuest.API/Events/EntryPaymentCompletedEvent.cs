using MediatR;

namespace FanQuest.API.Events
{
    public class EntryPaymentCompletedEvent : INotification
    {
        public Guid UserId { get; set; }
        public Guid QuestId { get; set; }
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
    }
}
