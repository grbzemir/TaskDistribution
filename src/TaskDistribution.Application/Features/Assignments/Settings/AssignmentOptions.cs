namespace TaskDistribution.Application.Features.Assignments.Settings;

public sealed class AssignmentOptions
{
    public const string SectionName = "Assignment";

    public string[] EnabledRules { get; set; } = [];

    public int HardTaskThreshold { get; set; } = 6;
}