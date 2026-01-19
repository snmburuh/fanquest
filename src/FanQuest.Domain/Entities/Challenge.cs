using FanQuest.Domain.Enums;

namespace FanQuest.Domain.Entities
{
    public abstract class Challenge
    {
        public Guid Id { get; protected set; }
        public Guid QuestId { get; protected set; }
        public string Title { get; protected set; }
        public int Points { get; protected set; }
        public DateTime OpensAt { get; protected set; }
        public DateTime ClosesAt { get; protected set; }
        public ChallengeType Type { get; protected set; }
        public string Metadata { get; protected set; } // JSON for extensibility

        protected Challenge() { } // EF Core

        protected Challenge(Guid questId, string title, int points, DateTime opensAt, DateTime closesAt, ChallengeType type)
        {
            Id = Guid.NewGuid();
            QuestId = questId;
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Points = points > 0 ? points : throw new ArgumentException("Points must be positive");
            OpensAt = opensAt;
            ClosesAt = closesAt;
            Type = type;
        }

        public bool IsOpen() => DateTime.UtcNow >= OpensAt && DateTime.UtcNow <= ClosesAt;
    }

    public class CheckInChallenge : Challenge
    {
        public string LocationName { get; private set; }

        protected CheckInChallenge() { } // EF Core

        public CheckInChallenge(Guid questId, string title, int points, DateTime opensAt, DateTime closesAt, string locationName)
            : base(questId, title, points, opensAt, closesAt, ChallengeType.CheckIn)
        {
            LocationName = locationName ?? throw new ArgumentNullException(nameof(locationName));
        }
    }

    public class TimedChallenge : Challenge
    {
        protected TimedChallenge() { } // EF Core

        public TimedChallenge(Guid questId, string title, int points, DateTime opensAt, DateTime closesAt)
            : base(questId, title, points, opensAt, closesAt, ChallengeType.Timed)
        {
        }
    }

    public class ReactionChallenge : Challenge
    {
        protected ReactionChallenge() { } // EF Core

        public ReactionChallenge(Guid questId, string title, int points, DateTime opensAt, DateTime closesAt)
            : base(questId, title, points, opensAt, closesAt, ChallengeType.Reaction)
        {
        }
    }
}
