namespace VeryBadDiceRoller.Services;

/// <summary>
/// MT19937 Mersenne Twister pseudo-random number generator.
/// Provides a statistically strong, long-period RNG suitable for dice simulation.
/// </summary>
public sealed class MersenneTwister
{
    private const int N = 624;
    private const int M = 397;
    private const uint MatrixA  = 0x9908b0dfU;
    private const uint UpperMask = 0x80000000U;
    private const uint LowerMask = 0x7fffffffU;

    private readonly uint[] _mt = new uint[N];
    private int _mti = N + 1;

    public MersenneTwister() : this((uint)Environment.TickCount) { }

    public MersenneTwister(uint seed) => Initialize(seed);

    private void Initialize(uint seed)
    {
        _mt[0] = seed;
        for (_mti = 1; _mti < N; _mti++)
            _mt[_mti] = 1812433253U * (_mt[_mti - 1] ^ (_mt[_mti - 1] >> 30)) + (uint)_mti;
    }

    /// <summary>Returns a random uint in [0, 2^32 - 1].</summary>
    public uint NextUInt32()
    {
        uint y;
        uint[] mag01 = [0x0U, MatrixA];

        if (_mti >= N)
        {
            int kk;
            for (kk = 0; kk < N - M; kk++)
            {
                y = (_mt[kk] & UpperMask) | (_mt[kk + 1] & LowerMask);
                _mt[kk] = _mt[kk + M] ^ (y >> 1) ^ mag01[y & 0x1U];
            }
            for (; kk < N - 1; kk++)
            {
                y = (_mt[kk] & UpperMask) | (_mt[kk + 1] & LowerMask);
                _mt[kk] = _mt[kk + (M - N)] ^ (y >> 1) ^ mag01[y & 0x1U];
            }
            y = (_mt[N - 1] & UpperMask) | (_mt[0] & LowerMask);
            _mt[N - 1] = _mt[M - 1] ^ (y >> 1) ^ mag01[y & 0x1U];
            _mti = 0;
        }

        y = _mt[_mti++];
        y ^= y >> 11;
        y ^= (y << 7)  & 0x9d2c5680U;
        y ^= (y << 15) & 0xefc60000U;
        y ^= y >> 18;
        return y;
    }

    /// <summary>
    /// Returns a random int in [minInclusive, maxExclusive) using rejection
    /// sampling to eliminate modulo bias.
    /// </summary>
    public int Next(int minInclusive, int maxExclusive)
    {
        if (maxExclusive <= minInclusive) return minInclusive;
        uint range = (uint)(maxExclusive - minInclusive);
        uint limit = uint.MaxValue - (uint.MaxValue % range);
        uint value;
        do { value = NextUInt32(); } while (value > limit);
        return minInclusive + (int)(value % range);
    }

    /// <summary>Returns a random double in [0.0, 1.0).</summary>
    public double NextDouble() => NextUInt32() * (1.0 / 4294967296.0);
}
