using FanQuest.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace FanQuest.Domain.Rules
{
    public class ChallengeRuleEngine
    {
        private readonly IEnumerable<IChallengeRule> _rules;
        private readonly ILogger<ChallengeRuleEngine> _logger;

        public ChallengeRuleEngine(IEnumerable<IChallengeRule> rules, ILogger<ChallengeRuleEngine> logger)
        {
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public RuleResult EvaluateAll(Challenge challenge, ChallengeContext context)
        {
            _logger.LogInformation("Evaluating challenge {ChallengeId} for user {UserId}",
                challenge.Id, context.UserId);

            foreach (var rule in _rules)
            {
                var result = rule.Evaluate(challenge, context);

                if (!result.IsSuccessful)
                {
                    _logger.LogWarning("Rule {RuleName} failed: {Reason}",
                        rule.GetType().Name, result.Reason);
                    return result;
                }
            }

            _logger.LogInformation("All rules passed for challenge {ChallengeId}", challenge.Id);

            return RuleResult.Success(challenge.Points);
        }
    }
}
