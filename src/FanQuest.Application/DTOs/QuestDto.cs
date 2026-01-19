using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanQuest.Application.DTOs
{

    public class QuestDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public decimal EntryFee { get; set; }
        public string Status { get; set; } = string.Empty;
        public int ChallengeCount { get; set; }
    }

    public class ChallengeDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Points { get; set; }
        public DateTime OpensAt { get; set; }
        public DateTime ClosesAt { get; set; }
        public string Type { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
    }


    public class LeaderboardEntryDto
    {
        public int Rank { get; set; }
        public Guid UserId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public int Score { get; set; }
    }

    public class RewardDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public int MinRank { get; set; }
        public int MaxRank { get; set; }
        public bool IsEligible { get; set; }
    }
}
