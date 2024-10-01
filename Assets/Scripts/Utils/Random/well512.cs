using System;

public class well512
{
    private const int W = 32;
    private const int R = 16;
    private const int P = 0;
    private const uint M1 = 13;
    private const uint M2 = 9;
    private const uint M3 = 5;

    private uint[] state = new uint[R];
    private uint index = 0;

    protected well512(uint seed)
    {
        SetSeed(seed);
    }

    private void SetSeed(uint seed)
    {
        var random = new Random((int)seed);
        for (var i = 0; i < R; i++)
        {
            state[i] = (uint)random.Next();
        }
        index = 0;
    }

    protected uint NextUInt()
    {
        uint a, b, c, d;
        a = state[index];
        c = state[(index + M1) & 15];
        b = a ^ c ^ (a << 16) ^ (c << 15);
        c = state[(index + M2) & 15];
        c ^= (c >> 11);
        a = state[index] = b ^ c;
        d = a ^ ((a << 5) & 0xda442d24U);
        index = (index + 15) & 15;
        a = state[index];
        state[index] = a ^ b ^ d ^ (a << 2) ^ (b << 18) ^ (c << 28);
        return state[index];
    }
}