using Godot;

namespace Game.Combat;

/// <summary>
/// Tracks current/max HP for whatever actor owns it and applies damage.
/// Deliberately knows nothing about hitboxes, input, AI or loot — those
/// are separate systems that call TakeDamage() or listen to the signals
/// below. Keeps this class attachable to Player, Dummy, and future
/// enemies/bosses without any of them needing to change.
/// </summary>
public partial class Health : Node
{
    [Signal] public delegate void DamagedEventHandler(float amount, float currentHealth);
    [Signal] public delegate void DiedEventHandler();

    [Export] public float MaxHealth = 100f;

    public float CurrentHealth { get; private set; }
    public bool IsDead => CurrentHealth <= 0f;

    public override void _Ready()
    {
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(DamageInfo damage)
    {
        if (IsDead || damage.Amount <= 0f)
        {
            return;
        }

        CurrentHealth = Mathf.Max(0f, CurrentHealth - damage.Amount);
        EmitSignal(SignalName.Damaged, damage.Amount, CurrentHealth);

        if (IsDead)
        {
            EmitSignal(SignalName.Died);
        }
    }

    public void Heal(float amount)
    {
        if (IsDead || amount <= 0f)
        {
            return;
        }

        CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);
        // Pode emitir sinal Healed se necessário futuramente
    }

    public void Revive(float percentage = 0.5f)
    {
        if (!IsDead)
        {
            return;
        }

        CurrentHealth = MaxHealth * Mathf.Clamp(percentage, 0f, 1f);
    }
}
