using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanQuest.Domain.Entities
{
    public class Participation
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public Guid QuestId { get; private set; }
        public int Score { get; private set; }
        public DateTime JoinedAt { get; private set; }

        private readonly List<Completion> _completions = new();
        public IReadOnlyCollection<Completion> Completions => _completions.AsReadOnly();

        protected Participation() { } // EF Core

        public Participation(Guid userId, Guid questId)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            QuestId = questId;
            Score = 0;
            JoinedAt = DateTime.UtcNow;
        }

        public void AddScore(int points)
        {
            if (points < 0) throw new ArgumentException("Points cannot be negative");
            Score += points;
        }

        public void AddCompletion(Completion completion)
        {
            if (completion == null) throw new ArgumentNullException(nameof(completion));
            _completions.Add(completion);
        }

        public bool HasCompleted(Guid challengeId) =>
            _completions.Any(c => c.ChallengeId == challengeId);
    }
}
