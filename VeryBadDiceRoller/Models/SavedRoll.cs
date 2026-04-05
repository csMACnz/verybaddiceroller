namespace VeryBadDiceRoller.Models;

public class ExpressionPart
{
    public int Count { get; set; }
    public int DieSize { get; set; }
}

public class SavedRoll
{
    public required string Name { get; set; }
    public required int Count { get; set; }
    public required int DieSize { get; set; }
    public required int Modifier { get; set; }
    public List<ExpressionPart>? ExpressionParts { get; set; }

    public bool IsExpression => ExpressionParts is { Count: > 0 };

    public string Notation =>
        IsExpression
            ? string.Join(" + ", ExpressionParts?.Select(p => $"{p.Count}d{p.DieSize}") ?? []) +
              (Modifier > 0 ? $"+{Modifier}" : Modifier < 0 ? $"{Modifier}" : "")
            : $"{Count}d{DieSize}" +
              (Modifier > 0 ? $"+{Modifier}" : Modifier < 0 ? $"{Modifier}" : "");
}
