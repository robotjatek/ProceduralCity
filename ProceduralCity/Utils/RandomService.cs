using System;

namespace ProceduralCity.Utils;

/// <summary>
/// A class responsible for global, reproducible pseudo-random data
/// </summary>
public class RandomService
{
    private readonly Random _random;

    public RandomService(int seed)
    {
        _random = new Random(seed);
    }

    public int Next()
    {
        return _random.Next();
    }

    public int Next(int max)
    {
        return _random.Next(max);
    }

    public int Next(int min, int max)
    {
        return _random.Next(min, max);
    }

    public double NextDouble()
    {
        return _random.NextDouble();
    }

    public bool FlipCoin()
    {
        return _random.Next(0, 10) % 2 == 0;
    }
}
