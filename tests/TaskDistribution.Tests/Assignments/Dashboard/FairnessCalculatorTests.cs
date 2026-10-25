using TaskDistribution.Application.Features.Dashboard;

using Xunit;

namespace TaskDistribution.Tests.Dashboard;

public sealed class FairnessCalculatorTests
{
    [Fact]
    public void CalculateJainIndex_WhenWorkloadsAreEqual_ReturnsOne()
    {
        var result = FairnessCalculator.CalculateJainIndex([12, 12, 12, 12]);

        Assert.Equal(1d, result, precision: 6);
    }

    [Fact]
    public void CalculateJainIndex_WhenOneDeveloperHasAllWork_ReturnsOneOverN()
    {
        var result = FairnessCalculator.CalculateJainIndex([40, 0, 0, 0, 0, 0, 0, 0]);

        Assert.Equal(1d / 8, result, precision: 6);
    }

    [Fact]
    public void CalculateJainIndex_ReturnsExpectedValueForKnownInput()
    {
        var result = FairnessCalculator.CalculateJainIndex([1, 2, 3]);

        Assert.Equal(36d / 42, result, precision: 6);
    }

    [Fact]
    public void CalculateJainIndex_WhenNoWorkIsAssigned_ReturnsOne()
    {
        Assert.Equal(1d, FairnessCalculator.CalculateJainIndex([0, 0, 0]));
    }

    [Fact]
    public void CalculateJainIndex_WhenThereAreNoDevelopers_ReturnsOne()
    {
        Assert.Equal(1d, FairnessCalculator.CalculateJainIndex([]));
    }

    [Fact]
    public void CalculateMaxSpreadPerLevel_ReturnsLargestDifferenceAcrossLevels()
    {
        IReadOnlyList<int>[] counts =
        [
            [2, 0, 1],
            [1, 0, 4],
            [2, 0, 2]
        ];

        Assert.Equal(3, FairnessCalculator.CalculateMaxSpreadPerLevel(counts));
    }

    [Fact]
    public void CalculateMaxSpreadPerLevel_WhenThereAreNoDevelopers_ReturnsZero()
    {
        Assert.Equal(0, FairnessCalculator.CalculateMaxSpreadPerLevel([]));
    }
}