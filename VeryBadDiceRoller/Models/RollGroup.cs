namespace VeryBadDiceRoller.Models;

public class RollGroup
{
    public required int Count { get; init; }
    public required int DieSize { get; init; }
    public required int Modifier { get; init; }
    public string? Label { get; init; }
    public required IReadOnlyList<DieResult> Results { get; init; }
    public required WeightMode Mode { get; init; }
    public required DateTime RolledAt { get; init; }

    public int Total => Results.Sum(r => r.Value) + Modifier;

    public string Notation =>
        $"{Count}d{DieSize}" +
        (Modifier > 0 ? $"+{Modifier}" : Modifier < 0 ? $"{Modifier}" : "");

    public string DisplayNotation =>
        string.IsNullOrEmpty(Label) ? Notation : $"{Label} ({Notation})";
}
