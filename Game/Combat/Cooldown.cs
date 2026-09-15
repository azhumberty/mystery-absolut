namespace Game.Combat;

/// <summary>
/// Minimal reusable cooldown timer. Deliberately NOT a Node — callers
/// (PlayerBasicAttack, PlayerDodge, and future skills) tick it manually
/// from their own _Process/_PhysicsProcess. Keeping it a plain value type
/// avoids adding more nodes to the scene tree just to track a countdown.
/// </summary>
public class Cooldown
{
    private double _remaining;

    public bool IsReady => _remaining <= 0.0;

    public void Tick(double delta)
    {
        if (_remaining > 0.0)
        {
            _remaining -= delta;
        }
    }

    public void Start(double duration)
    {
        _remaining = duration;
    }
}
