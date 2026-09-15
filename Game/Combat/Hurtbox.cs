using Godot;

namespace Game.Combat;

/// <summary>
/// Area2D that represents "this part of the actor can be hit". Forwards
/// any DamageInfo it receives to the actor's Health component, after
/// applying whatever defensive state is currently active. Knows nothing
/// about who is attacking, or about dodge/block input — those are
/// separate components (ex: PlayerDodge, PlayerBlock) that just set the
/// properties below; Hurtbox only knows how to react to them:
///
///  - Invulnerable: no damage at all (dodge i-frames).
///  - ParryWindowActive: no damage at all, but distinct from
///    Invulnerable — it emits Parried instead of silently absorbing the
///    hit, so a future stagger/counter-attack system can react
///    differently to "parried" vs. "simply missed". This is the "base de
///    parry" asked for in Etapa 3: no stagger/counter yet, just the
///    signal a later system can hook into.
///  - DamageReductionRatio: partial reduction (blocking).
/// </summary>
public partial class Hurtbox : Area2D
{
    [Signal] public delegate void ParriedEventHandler(float amount, Node2D source);

    [Export] public NodePath HealthPath;

    public bool Invulnerable { get; set; }
    public bool ParryWindowActive { get; set; }
    public float DamageReductionRatio { get; set; }

    private Health _health;

    public override void _Ready()
    {
        _health = HealthPath != null && !HealthPath.IsEmpty
            ? GetNode<Health>(HealthPath)
            : GetParent().GetNodeOrNull<Health>("Health");
    }

    public void ReceiveDamage(DamageInfo damage)
    {
        if (ParryWindowActive)
        {
            EmitSignal(SignalName.Parried, damage.Amount, damage.Source);
            return;
        }

        if (Invulnerable)
        {
            return;
        }

        float ratio = 1f - Mathf.Clamp(DamageReductionRatio, 0f, 1f);
        float reducedAmount = damage.Amount * ratio;
        if (reducedAmount <= 0f)
        {
            return;
        }

        _health?.TakeDamage(new DamageInfo(reducedAmount, damage.Source));
    }
}
