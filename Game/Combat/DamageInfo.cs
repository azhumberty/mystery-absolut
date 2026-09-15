using Godot;

namespace Game.Combat;

/// <summary>
/// Plain data describing a single instance of damage: how much, and who
/// (or what) caused it. Kept as a simple readonly struct, with no
/// knowledge of hitboxes, resistances or how it gets applied, so future
/// systems (skills, status effects, crafting-affected damage) can all
/// produce a DamageInfo the same way without depending on combat
/// internals.
/// </summary>
public readonly struct DamageInfo
{
    public float Amount { get; }
    public Node2D Source { get; }

    public DamageInfo(float amount, Node2D source)
    {
        Amount = amount;
        Source = source;
    }
}
