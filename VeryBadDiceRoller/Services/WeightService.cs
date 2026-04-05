namespace VeryBadDiceRoller.Services;

using VeryBadDiceRoller.Models;

/// <summary>
/// Tracks the automatic advantage/disadvantage weighting.
/// State is session-local (per WASM instance / browser tab).
/// Mode shifts when either a time threshold or a roll-count threshold is reached.
/// Both thresholds are randomised on every shift so the pattern is not predictable.
/// </summary>
public class WeightService
{
    private readonly Random _rng = new();

    private WeightMode _currentMode;
    private DateTime _lastShift;
    private TimeSpan _timeThreshold;
    private int _rollThreshold;
    private int _rollsSinceShift;

    public WeightService()
    {
        // Random initial mode
        _currentMode = _rng.Next(2) == 0 ? WeightMode.Advantage : WeightMode.Disadvantage;
        _lastShift = DateTime.UtcNow;
        SetNewThresholds();
    }

    private void SetNewThresholds()
    {
        // Time threshold: 2–5 minutes (120–300 seconds)
        _timeThreshold = TimeSpan.FromSeconds(_rng.Next(120, 301));
        // Roll-count threshold: 10–25 rolls
        _rollThreshold = _rng.Next(10, 26);
    }

    public WeightMode GetCurrentMode()
    {
        CheckForShift();
        return _currentMode;
    }

    /// <summary>Called by DiceService after every roll to update the shift counter.</summary>
    public void RecordRoll()
    {
        _rollsSinceShift++;
        CheckForShift();
    }

    private void CheckForShift()
    {
        var elapsed = DateTime.UtcNow - _lastShift;
        if (elapsed >= _timeThreshold || _rollsSinceShift >= _rollThreshold)
        {
            _currentMode = _currentMode == WeightMode.Advantage
                ? WeightMode.Disadvantage
                : WeightMode.Advantage;
            _lastShift = DateTime.UtcNow;
            _rollsSinceShift = 0;
            SetNewThresholds();
        }
    }

    /// <summary>Seconds remaining before the next scheduled shift (time-based).</summary>
    public TimeSpan GetTimeUntilNextChange()
    {
        var elapsed = DateTime.UtcNow - _lastShift;
        var remaining = _timeThreshold - elapsed;
        return remaining < TimeSpan.Zero ? TimeSpan.Zero : remaining;
    }

    /// <summary>0..1 progress through the current time threshold.</summary>
    public double GetCurrentPhaseProgress()
    {
        var elapsed = (DateTime.UtcNow - _lastShift).TotalSeconds;
        return Math.Min(1.0, elapsed / _timeThreshold.TotalSeconds);
    }

    public int RollsSinceShift => _rollsSinceShift;
    public int RollThreshold => _rollThreshold;
}
