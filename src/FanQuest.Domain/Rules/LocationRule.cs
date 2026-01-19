using FanQuest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanQuest.Domain.Rules
{
    public class LocationRule : IChallengeRule
    {
        public RuleResult Evaluate(Challenge challenge, ChallengeContext context)
        {
            // Phase 1: Soft validation - just check presence of coordinates
            if (!context.Latitude.HasValue || !context.Longitude.HasValue)
                return RuleResult.Failure("Location data required");

            // Future: Add geofencing/distance calculation
            return RuleResult.Success(0);
        }
    }
}
