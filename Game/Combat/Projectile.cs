using Godot;

namespace Game.Combat;

public partial class Projectile : Area2D
{
    [Export] public float Speed { get; set; } = 300f;
    [Export] public float Damage { get; set; } = 25f;
    [Export] public float Lifetime { get; set; } = 2f;

    public Vector2 Direction { get; set; }
    public Node2D Owner2D { get; set; }

    private float _age = 0f;

    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;
        BodyEntered += OnBodyEntered;
    }

    public override void _PhysicsProcess(double delta)
    {
        _age += (float)delta;
        if (_age >= Lifetime)
        {
            QueueFree();
            return;
        }

        GlobalPosition += Direction * Speed * (float)delta;
    }

    private void OnAreaEntered(Area2D area)
    {
        if (area is Hurtbox hurtbox && hurtbox.Owner != Owner2D)
        {
            // Note: In our current setup, Combatant handles teams.
            // Let's assume hitting any hurtbox not owned by us is a hit for simplicity.
            var targetCombatant = hurtbox.GetParent().GetNodeOrNull<Combatant>("Combatant");
            var myCombatant = Owner2D?.GetNodeOrNull<Combatant>("Combatant");
            
            if (targetCombatant != null && myCombatant != null && targetCombatant.Team == myCombatant.Team)
                return;

            hurtbox.ReceiveDamage(new DamageInfo(Damage, Owner2D));
            QueueFree();
        }
    }

    private void OnBodyEntered(Node2D body)
    {
        // Hit a wall
        if (body is TileMapLayer || body is StaticBody2D)
        {
            QueueFree();
        }
    }
}
