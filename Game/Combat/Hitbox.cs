using Godot;

namespace Game.Combat;

/// <summary>
/// Area2D that represents "this deals damage while active". Disabled
/// (Monitoring = false) by default; whoever owns the attack (ex:
/// PlayerBasicAttack) calls Activate() to enable it for a short window
/// and Deactivate() when the window ends. Tracks which Hurtboxes it has
/// already hit during the current activation (_alreadyHit) so a single
/// swing can't multi-hit the same target every physics frame it overlaps.
/// </summary>
public partial class Hitbox : Area2D
{
    [Export] public float Damage = 10f;

    /// <summary>
    /// Who owns this hitbox (ex: the Player CharacterBody2D). Named
    /// Owner2D, not Owner, to avoid colliding with Godot's built-in
    /// Node.Owner (scene-ownership for saving), which is a different
    /// concept entirely.
    /// </summary>
    public Node2D Owner2D { get; set; }

    private readonly System.Collections.Generic.HashSet<Hurtbox> _alreadyHit = new();

    public override void _Ready()
    {
        Monitoring = false;
        AreaEntered += OnAreaEntered;
    }

    public void Activate()
    {
        _alreadyHit.Clear();
        SetDeferred(Area2D.PropertyName.Monitoring, true);
    }

    public void Deactivate()
    {
        SetDeferred(Area2D.PropertyName.Monitoring, false);
    }

    private void OnAreaEntered(Area2D area)
    {
        if (area is not Hurtbox hurtbox || !_alreadyHit.Add(hurtbox))
        {
            return;
        }

        hurtbox.ReceiveDamage(new DamageInfo(Damage, Owner2D));
    }
}
