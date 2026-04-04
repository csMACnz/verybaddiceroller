namespace VeryBadDiceRoller.Models;

public class SavedRoll
{
    public required string Name { get; set; }
    public required int Count { get; set; }
    public required int DieSize { get; set; }
    public required int Modifier { get; set; }

    public string Notation =>
        $"{Count}d{DieSize}" +
        (Modifier > 0 ? $"+{Modifier}" : Modifier < 0 ? $"{Modifier}" : "");
}
