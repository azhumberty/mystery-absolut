using Godot;
using Game.Combat;

namespace Game.Actors.Player;

/// <summary>
/// Holding "block" reduces incoming damage (base block, via
/// Hurtbox.DamageReductionRatio). Etapa 3 also asks for a "base de
/// parry" — not a full parry system (no attacker stagger/knockback yet;
/// those depend on systems that don't exist until later etapas), just
/// the foundation: a short window right after block starts where a hit
/// is fully negated and reported via Hurtbox.Parried, distinct from a
/// normal partial block. A future etapa can react to Parried (stagger
/// the attacker, trigger a counter-attack) without touching this file or
/// Hurtbox.
/// </summary>
public partial class PlayerBlock : Node
{
    [Export] public float BlockDamageReduction = 0.5f;
    [Export] public float ParryWindowDuration = 0.15f;

    private Hurtbox _hurtbox;
    private double _parryWindowRemaining;

    public bool IsBlocking { get; private set; }

    public override void _Ready()
    {
        _hurtbox = GetParent().GetNode<Hurtbox>("Hurtbox");
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("block"))
        {
            _parryWindowRemaining = ParryWindowDuration;
        }
        else if (_parryWindowRemaining > 0.0)
        {
            _parryWindowRemaining -= delta;
        }

        IsBlocking = Input.IsActionPressed("block");
        _hurtbox.ParryWindowActive = _parryWindowRemaining > 0.0;
        _hurtbox.DamageReductionRatio = IsBlocking ? BlockDamageReduction : 0f;
    }
}
