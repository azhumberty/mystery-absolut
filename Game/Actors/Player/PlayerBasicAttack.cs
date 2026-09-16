using Godot;
using Game.Combat;

namespace Game.Actors.Player;

/// <summary>
/// Reads the "attack" action and swings the player's AttackHitbox
/// (sibling Area2D under Player) in the direction the player is
/// currently facing. Deliberately separate from PlayerController (which
/// only owns movement) and from Hitbox (which only owns "what happens
/// when this overlaps a Hurtbox") — this script is just the glue: input
/// → cooldown → position the hitbox → activate it for a short window →
/// deactivate it.
/// </summary>
public partial class PlayerBasicAttack : Node
{
    [Export] public float Damage = 15f;
    [Export] public float AttackDuration = 0.15f;
    [Export] public float CooldownDuration = 0.45f;
    [Export] public float AttackRange = 28f;

    private PlayerController _player;
    private Hitbox _hitbox;
    private readonly Cooldown _cooldown = new();
    private double _activeTimeRemaining;

    public override void _Ready()
    {
        _player = GetParent<PlayerController>();
        _hitbox = GetParent().GetNode<Hitbox>("AttackHitbox");
        _hitbox.Owner2D = _player;
        _hitbox.Damage = Damage;
    }

    public override void _Process(double delta)
    {
        _cooldown.Tick(delta);

        if (_activeTimeRemaining > 0.0)
        {
            _activeTimeRemaining -= delta;
            if (_activeTimeRemaining <= 0.0)
            {
                _hitbox.Deactivate();
            }
        }

        if (_cooldown.IsReady && Input.IsActionJustPressed("attack"))
        {
            TriggerAttack();
        }
    }

    private void TriggerAttack()
    {
        _hitbox.Position = _player.FacingDirection * AttackRange;
        _hitbox.Activate();

        var audio = GetNodeOrNull<Game.Core.AudioManager>("/root/AudioManager");
        audio?.PlayAttack();

        _activeTimeRemaining = AttackDuration;
        _cooldown.Start(CooldownDuration);
    }
}
