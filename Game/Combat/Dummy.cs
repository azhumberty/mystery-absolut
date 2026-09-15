using Godot;

namespace Game.Combat;

/// <summary>
/// Stationary target used only to prove Health/Hitbox/Hurtbox work
/// end-to-end. Not an enemy: no AI, no death handling, no loot, no
/// respawn — that's Etapa 4+. Gives simple visual feedback (fades as it
/// loses HP) and prints to the console, purely so this can be verified by
/// eye/log without building a proper "Hit Feedback" system (that's a
/// separate future item, not in scope for Etapa 2).
/// </summary>
public partial class Dummy : StaticBody2D
{
    private Health _health;
    private Polygon2D _visual;

    public override void _Ready()
    {
        _health = GetNode<Health>("Health");
        _visual = GetNode<Polygon2D>("Visual");
        _health.Damaged += OnDamaged;
        _health.Died += OnDied;
    }

    private void OnDamaged(float amount, float currentHealth)
    {
        float ratio = _health.MaxHealth > 0f ? currentHealth / _health.MaxHealth : 0f;
        _visual.Modulate = new Color(1f, 1f, 1f, Mathf.Clamp(0.25f + 0.75f * ratio, 0.25f, 1f));
        GD.Print($"Dummy took {amount} damage, {currentHealth}/{_health.MaxHealth} HP remaining.");
    }

    private void OnDied()
    {
        GD.Print("Dummy defeated (Etapa 2 test only — no death/respawn system yet).");
    }
}
