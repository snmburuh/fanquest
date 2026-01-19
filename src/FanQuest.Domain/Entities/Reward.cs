using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanQuest.Domain.Entities
{
    public class Reward
    {
        public Guid Id { get; private set; }
        public Guid QuestId { get; private set; }
        public string Name { get; private set; }
        public decimal Value { get; private set; }
        public int MinRank { get; private set; }
        public int MaxRank { get; private set; }

        protected Reward() { } // EF Core

        public Reward(Guid questId, string name, decimal value, int minRank, int maxRank)
        {
            if (minRank <= 0 || maxRank < minRank)
                throw new ArgumentException("Invalid rank range");

            Id = Guid.NewGuid();
            QuestId = questId;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = value;
            MinRank = minRank;
            MaxRank = maxRank;
        }

        public bool IsEligible(int rank) => rank >= MinRank && rank <= MaxRank;
    }
}
