using FanQuest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanQuest.Domain.Rules
{
    public class SingleCompletionRule : IChallengeRule
    {
        private readonly Func<Guid, Guid, bool> _hasCompletedChecker;

        public SingleCompletionRule(Func<Guid, Guid, bool> hasCompletedChecker)
        {
            _hasCompletedChecker = hasCompletedChecker ?? throw new ArgumentNullException(nameof(hasCompletedChecker));
        }

        public RuleResult Evaluate(Challenge challenge, ChallengeContext context)
        {
            if (_hasCompletedChecker(context.UserId, challenge.Id))
                return RuleResult.Failure("Challenge already completed");

            return RuleResult.Success(0);
        }
    }
}
