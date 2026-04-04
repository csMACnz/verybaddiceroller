namespace VeryBadDiceRoller.Models;

public record DieResult(int DieSize, int RollA, int RollB, WeightMode Mode)
{
    public int Value => Mode == WeightMode.Advantage
        ? Math.Max(RollA, RollB)
        : Math.Min(RollA, RollB);

    public int DroppedValue => Mode == WeightMode.Advantage
        ? Math.Min(RollA, RollB)
        : Math.Max(RollA, RollB);

    public bool IsBiased => RollA != RollB;
}
