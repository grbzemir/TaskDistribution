namespace TaskDistribution.Application.Features.Dashboard;

public static class FairnessCalculator
{
    public static double CalculateJainIndex(IReadOnlyCollection<int> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        if (values.Count == 0)
        {
            return 1d;
        }

        double sum = values.Sum();
        var sumOfSquares = values.Sum(value => (double)value * value);

        if (sumOfSquares == 0)
        {
            return 1d;
        }

        return sum * sum / (values.Count * sumOfSquares);
    }

    public static int CalculateMaxSpreadPerLevel(IReadOnlyCollection<IReadOnlyList<int>> countsByDeveloper)
    {
        ArgumentNullException.ThrowIfNull(countsByDeveloper);

        if (countsByDeveloper.Count == 0)
        {
            return 0;
        }

        var levelCount = countsByDeveloper.First().Count;

        return Enumerable.Range(0, levelCount)
            .Select(level =>
            {
                var countsAtLevel = countsByDeveloper.Select(counts => counts[level]).ToList();
                return countsAtLevel.Max() - countsAtLevel.Min();
            })
            .DefaultIfEmpty(0)
            .Max();
    }
}