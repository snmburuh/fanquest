using FanQuest.Domain.Enums;

namespace FanQuest.Domain.Entities
{
    public class Quest
    {
        public Guid Id { get; private set; }
        public Guid TenantId { get; private set; }
        public string Name { get; private set; }
        public string City { get; private set; }
        public DateTime StartsAt { get; private set; }
        public DateTime EndsAt { get; private set; }
        public decimal EntryFee { get; private set; }
        public QuestStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private readonly List<Challenge> _challenges = new();
        public IReadOnlyCollection<Challenge> Challenges => _challenges.AsReadOnly();

        protected Quest() { } // EF Core

        public Quest(string name, string city, DateTime startsAt, DateTime endsAt, decimal entryFee, Guid tenantId)
        {
            if (startsAt >= endsAt)
                throw new ArgumentException("Start time must be before end time");

            Id = Guid.NewGuid();
            TenantId = tenantId;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            City = city ?? throw new ArgumentNullException(nameof(city));
            StartsAt = startsAt;
            EndsAt = endsAt;
            EntryFee = entryFee;
            Status = QuestStatus.Draft;
            CreatedAt = DateTime.UtcNow;
        }

        public Quest(string name, string city, DateTime startsAt, DateTime endsAt, decimal entryFee)
        {
            Name = name;
            City = city;
            StartsAt = startsAt;
            EndsAt = endsAt;
            EntryFee = entryFee;
        }

        public void Publish()
        {
            if (Status != QuestStatus.Draft)
                throw new InvalidOperationException("Only draft quests can be published");
            Status = QuestStatus.Live;
        }

        public void Complete()
        {
            if (Status != QuestStatus.Live)
                throw new InvalidOperationException("Only live quests can be completed");
            Status = QuestStatus.Completed;
        }

        public void AddChallenge(Challenge challenge)
        {
            if (challenge == null) throw new ArgumentNullException(nameof(challenge));
            _challenges.Add(challenge);
        }

        public bool IsActive() => Status == QuestStatus.Live && DateTime.UtcNow < EndsAt;
    }
}
