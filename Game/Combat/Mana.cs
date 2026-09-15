using Godot;

namespace Game.Combat;

public partial class Mana : Node
{
    [Signal] public delegate void ManaChangedEventHandler(float current, float max);

    [Export] public float MaxMana { get; set; } = 100f;
    [Export] public float ManaRegenPerSecond { get; set; } = 5f;

    public float CurrentMana { get; private set; }

    public override void _Ready()
    {
        CurrentMana = MaxMana;
    }

    public override void _Process(double delta)
    {
        if (CurrentMana < MaxMana)
        {
            CurrentMana = Mathf.Min(MaxMana, CurrentMana + ManaRegenPerSecond * (float)delta);
            EmitSignal(SignalName.ManaChanged, CurrentMana, MaxMana);
        }
    }

    public bool TryConsume(float amount)
    {
        if (CurrentMana >= amount)
        {
            CurrentMana -= amount;
            EmitSignal(SignalName.ManaChanged, CurrentMana, MaxMana);
            return true;
        }
        return false;
    }
}
