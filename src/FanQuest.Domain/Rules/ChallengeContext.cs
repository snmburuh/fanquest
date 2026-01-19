using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanQuest.Domain.Rules
{
    public class ChallengeContext
    {
        public Guid UserId { get; set; }
        public Guid QuestId { get; set; }
        public Guid ChallengeId { get; set; }
        public DateTime Timestamp { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Payload { get; set; }
    }
}
