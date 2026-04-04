namespace VeryBadDiceRoller.Services;

using VeryBadDiceRoller.Models;

public class DiceService
{
    private readonly WeightService _weightService;
    private readonly Random _sysRng = new();
    private readonly MersenneTwister _mtRng = new();

    public DiceService(WeightService weightService) => _weightService = weightService;

    // ── Public raw helpers (bypass weighting) ────────────────────────────────

    /// <summary>Roll a single die without any weighting (used for D&amp;D generator).</summary>
    public int RollUnweighted(int dieSize, bool useMT = true) =>
        useMT ? _mtRng.Next(1, dieSize + 1) : _sysRng.Next(1, dieSize + 1);

    /// <summary>Roll a single die using the selected algorithm (used by randomness test).</summary>
    public int RollRawWithAlgorithm(int dieSize, bool useMT) =>
        useMT ? _mtRng.Next(1, dieSize + 1) : _sysRng.Next(1, dieSize + 1);

    // ── Main roll ────────────────────────────────────────────────────────────

    /// <summary>
    /// Rolls <paramref name="count"/> dice of size <paramref name="dieSize"/>
    /// applying the internal weighting bias and the user's roll mode.
    /// </summary>
    public RollGroup Roll(
        int count, int dieSize, int modifier,
        string? label = null,
        UserRollMode userMode = UserRollMode.Normal,
        bool modPerDie = false,
        bool useMT = true)
    {
        var internalMode = _weightService.GetCurrentMode();

        var results = Enumerable.Range(0, count)
            .Select(_ => RollOneDie(dieSize, internalMode, userMode, useMT))
            .ToList();

        _weightService.RecordRoll();

        return new RollGroup
        {
            Count       = count,
            DieSize     = dieSize,
            Modifier    = modifier,
            ModPerDie   = modPerDie,
            Label       = label,
            Results     = results,
            InternalMode = internalMode,
            UserMode    = userMode,
            RolledAt    = DateTime.UtcNow
        };
    }

    // ── Internal per-die logic (§12.4 table) ────────────────────────────────

    private DieResult RollOneDie(int dieSize, WeightMode internalMode, UserRollMode userMode, bool useMT)
    {
        if (userMode == UserRollMode.Normal)
        {
            // Normal user roll: internal bias selects higher or lower of two raw rolls.
            var (val, dropped) = InternallyBiasedRollWithDropped(dieSize, internalMode, useMT);
            return new DieResult { DieSize = dieSize, Value = val, InternalDropped = dropped };
        }
        else
        {
            // Advantage or Disadvantage user roll:
            // roll two dice, each internally biased; then apply user's mode on the two results.
            var (a, droppedA) = InternallyBiasedRollWithDropped(dieSize, internalMode, useMT);
            var (b, droppedB) = InternallyBiasedRollWithDropped(dieSize, internalMode, useMT);

            int kept, userDropped;
            if (userMode == UserRollMode.Advantage)
            {
                kept = Math.Max(a, b);
                userDropped = Math.Min(a, b);
            }
            else
            {
                kept = Math.Min(a, b);
                userDropped = Math.Max(a, b);
            }

            // Report internal dropped from whichever die was kept
            int? internalDropped = (kept == a) ? droppedA : droppedB;

            return new DieResult
            {
                DieSize = dieSize,
                Value = kept,
                InternalDropped = internalDropped,
                UserDropped = userDropped
            };
        }
    }

    private (int value, int? dropped) InternallyBiasedRollWithDropped(int dieSize, WeightMode mode, bool useMT)
    {
        int a = useMT ? _mtRng.Next(1, dieSize + 1) : _sysRng.Next(1, dieSize + 1);
        int b = useMT ? _mtRng.Next(1, dieSize + 1) : _sysRng.Next(1, dieSize + 1);
        int kept    = mode == WeightMode.Advantage ? Math.Max(a, b) : Math.Min(a, b);
        int dropped = mode == WeightMode.Advantage ? Math.Min(a, b) : Math.Max(a, b);
        return (kept, a == b ? null : dropped);
    }
}
