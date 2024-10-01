using System;
using UnityEngine;
using Random = System.Random;

public sealed class NewRandom : well512
{
    public NewRandom() : base((uint)Environment.TickCount) { }

    public NewRandom(uint seed) : base(seed) { }

    public int Next()
    {
        return (int)(NextUInt() >> 1);
    }

    public int Next(int minValue, int maxValue)
    {
        return minValue + (int)(NextDouble() * (maxValue - minValue));
    }

    public double NextDouble()
    {
        return NextUInt() * (1.0 / 4294967295.0);
    }

    public float Range(float minValue, float maxValue)
    {
        return (float)(NextDouble() * (maxValue - minValue) + minValue);
    }

    public int Range(int minValue, int maxValue)
    {
        return Next(minValue, maxValue);
    }
}