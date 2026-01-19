using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanQuest.Domain.Entities
{
    public class Completion
    {
        public Guid Id { get; private set; }
        public Guid ParticipationId { get; private set; }
        public Guid ChallengeId { get; private set; }
        public int PointsAwarded { get; private set; }
        public DateTime CompletedAt { get; private set; }

        protected Completion() { } // EF Core

        public Completion(Guid participationId, Guid challengeId, int pointsAwarded)
        {
            Id = Guid.NewGuid();
            ParticipationId = participationId;
            ChallengeId = challengeId;
            PointsAwarded = pointsAwarded;
            CompletedAt = DateTime.UtcNow;
        }
    }
}
