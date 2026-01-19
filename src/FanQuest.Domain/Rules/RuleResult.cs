using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanQuest.Domain.Rules
{
    public class RuleResult
    {
        public bool IsSuccessful { get; set; }
        public int PointsAwarded { get; set; }
        public string Reason { get; set; } = string.Empty;

        public static RuleResult Success(int points) => new()
        {
            IsSuccessful = true,
            PointsAwarded = points
        };

        public static RuleResult Failure(string reason) => new()
        {
            IsSuccessful = false,
            PointsAwarded = 0,
            Reason = reason
        };
    }
}
