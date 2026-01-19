using FanQuest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanQuest.Domain.Rules
{
    public interface IChallengeRule
    {
        RuleResult Evaluate(Challenge challenge, ChallengeContext context);
    }

}
