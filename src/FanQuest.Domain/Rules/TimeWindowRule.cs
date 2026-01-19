using FanQuest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanQuest.Domain.Rules
{
    public class TimeWindowRule : IChallengeRule
    {
        public RuleResult Evaluate(Challenge challenge, ChallengeContext context)
        {
            if (context.Timestamp < challenge.OpensAt)
                return RuleResult.Failure("Challenge not yet open");

            if (context.Timestamp > challenge.ClosesAt)
                return RuleResult.Failure("Challenge has closed");

            return RuleResult.Success(0); // Pass-through, points added by orchestrator
        }
    }
}
