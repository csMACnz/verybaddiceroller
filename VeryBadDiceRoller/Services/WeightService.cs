namespace VeryBadDiceRoller.Services;

using VeryBadDiceRoller.Models;

/// <summary>
/// Tracks the automatic advantage/disadvantage weighting that cycles over time.
/// Every 30 seconds the mode alternates: 15 s of Advantage, 15 s of Disadvantage.
/// The cycle is anchored to UTC midnight so all users share the same phase.
/// </summary>
public class WeightService
{
    // Full cycle length in seconds (15 s per mode).
    private const double PeriodSeconds = 30.0;

    public WeightMode GetCurrentMode() => GetModeAt(DateTime.UtcNow);

    public WeightMode GetModeAt(DateTime utcTime)
    {
        var phase = GetPhaseSeconds(utcTime);
        return phase < PeriodSeconds / 2 ? WeightMode.Advantage : WeightMode.Disadvantage;
    }

    /// <summary>Returns seconds remaining before the mode switches.</summary>
    public TimeSpan GetTimeUntilNextChange()
    {
        var phase = GetPhaseSeconds(DateTime.UtcNow);
        double remaining = phase < PeriodSeconds / 2
            ? PeriodSeconds / 2 - phase
            : PeriodSeconds - phase;
        return TimeSpan.FromSeconds(remaining);
    }

    /// <summary>Returns 0..1 progress through the current half-period.</summary>
    public double GetCurrentPhaseProgress()
    {
        var phase = GetPhaseSeconds(DateTime.UtcNow);
        return phase < PeriodSeconds / 2
            ? phase / (PeriodSeconds / 2)
            : (phase - PeriodSeconds / 2) / (PeriodSeconds / 2);
    }

    private static double GetPhaseSeconds(DateTime utcTime)
        => utcTime.TimeOfDay.TotalSeconds % PeriodSeconds;
}
