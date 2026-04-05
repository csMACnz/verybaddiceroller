namespace VeryBadDiceRoller.Models;

/// <summary>
/// Represents one die roll, accounting for the internal weighting bias.
/// For normal user rolls the internal engine still rolls two values; for
/// advantage/disadvantage the user also gets a second (user-visible) die.
/// </summary>
public class DieResult
{
    /// <summary>Face count of this die (e.g. 6 for a d6).</summary>
    public required int DieSize { get; init; }

    /// <summary>The value shown to the user.</summary>
    public required int Value { get; init; }

    /// <summary>
    /// The value discarded by the internal weighting engine (hidden from user).
    /// Null when the two internal rolls happened to be equal.
    /// </summary>
    public int? InternalDropped { get; init; }

    /// <summary>
    /// For explicit Advantage/Disadvantage user rolls, the second die's final value
    /// (after its own internal weighting). Null for Normal user rolls.
    /// </summary>
    public int? UserDropped { get; init; }

    /// <summary>True when the internal bias actually changed the outcome.</summary>
    public bool IsBiased => InternalDropped.HasValue;
}
