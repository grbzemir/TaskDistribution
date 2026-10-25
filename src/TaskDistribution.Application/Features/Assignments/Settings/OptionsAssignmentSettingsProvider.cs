using Microsoft.Extensions.Options;

using TaskDistribution.Domain.Constants;

namespace TaskDistribution.Application.Features.Assignments.Settings;

public sealed class OptionsAssignmentSettingsProvider(IOptionsMonitor<AssignmentOptions> options)
    : IAssignmentSettingsProvider
{
    public Task<AssignmentSettings> GetAsync(CancellationToken cancellationToken = default)
    {
        var current = options.CurrentValue;

        var enabledRuleKeys = current.EnabledRules
            .Where(key => !string.IsNullOrWhiteSpace(key))
            .Select(key => key.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var hardTaskThreshold = Math.Clamp(
            current.HardTaskThreshold,
            DifficultyLevel.Min,
            DifficultyLevel.Max);

        return Task.FromResult(new AssignmentSettings(enabledRuleKeys, hardTaskThreshold));
    }
}