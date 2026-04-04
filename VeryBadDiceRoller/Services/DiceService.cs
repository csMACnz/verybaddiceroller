namespace VeryBadDiceRoller.Services;

using VeryBadDiceRoller.Models;

public class DiceService
{
    private readonly WeightService _weightService;
    private readonly Random _rng = new();

    public DiceService(WeightService weightService) => _weightService = weightService;

    /// <summary>
    /// Rolls <paramref name="count"/> dice of size <paramref name="dieSize"/>.
    /// Each die is rolled twice; the result kept depends on the current WeightMode.
    /// </summary>
    public RollGroup Roll(int count, int dieSize, int modifier = 0, string? label = null)
    {
        var mode = _weightService.GetCurrentMode();

        var results = Enumerable.Range(0, count)
            .Select(_ => new DieResult(
                DieSize: dieSize,
                RollA: _rng.Next(1, dieSize + 1),
                RollB: _rng.Next(1, dieSize + 1),
                Mode: mode))
            .ToList();

        return new RollGroup
        {
            Count = count,
            DieSize = dieSize,
            Modifier = modifier,
            Label = label,
            Results = results,
            Mode = mode,
            RolledAt = DateTime.UtcNow
        };
    }
}
