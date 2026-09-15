using Godot;

namespace Game.Combat;

/// <summary>
/// Marker/aggregator component: ties an actor's combat identity (team)
/// to its Health, so hitboxes and future systems (AI targeting, status
/// effects) have one place to ask "who/what is this" without reaching
/// into unrelated nodes. Expects a sibling node literally named "Health"
/// with a Health.cs script — that convention is simple enough not to
/// need an exported NodePath override for this foundation stage.
/// </summary>
public partial class Combatant : Node
{
    public enum CombatTeam
    {
        Player,
        Enemy,
        Neutral
    }

    [Export] public CombatTeam Team = CombatTeam.Neutral;

    public Health Health { get; private set; }

    public override void _Ready()
    {
        Health = GetParent()?.GetNodeOrNull<Health>("Health");
    }
}
