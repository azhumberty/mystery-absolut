using Godot;

namespace Game.Inventory;

public partial class Gold : Node
{
    [Signal] public delegate void GoldChangedEventHandler(int currentGold);

    [Export] public int CurrentGold { get; private set; } = 0;

    public void AddGold(int amount)
    {
        if (amount <= 0) return;
        CurrentGold += amount;
        EmitSignal(SignalName.GoldChanged, CurrentGold);
    }

    public bool TrySpendGold(int amount)
    {
        if (amount <= 0 || CurrentGold < amount) return false;
        
        CurrentGold -= amount;
        EmitSignal(SignalName.GoldChanged, CurrentGold);
        return true;
    }
}
