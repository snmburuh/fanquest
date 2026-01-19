using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanQuest.Domain.Enums
{
    public enum QuestStatus
    {
        Draft = 0,
        Live = 1,
        Completed = 2
    }


    public enum ChallengeType
    {
        CheckIn = 0,
        Timed = 1,
        Reaction = 2
    }


    public enum PaymentType
    {
        Entry = 0,
        Reward = 1
    }


    public enum PaymentStatus
    {
        Pending = 0,
        Success = 1,
        Failed = 2
    }
}
