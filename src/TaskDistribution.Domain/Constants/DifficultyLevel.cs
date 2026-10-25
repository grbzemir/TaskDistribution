using System;
using System.Collections.Generic;
using System.Text;

using TaskDistribution.Domain.Exceptions;

namespace TaskDistribution.Domain.Constants;

public static class DifficultyLevel
{
    public const int Min = 1;
    public const int Max = 8;

    public static bool IsValid(int value) => value is >= Min and <= Max;

    public static void EnsureValid(int value)
    {
        if (!IsValid(value))
        {
            throw new DomainException($"Difficulty must be between {Min} and {Max}.");
        }
    }
}
