using Godot;
using Game.Combat;

namespace Game.Actors.Player;

/// <summary>
/// Dash in the player's current movement direction (falls back to facing
/// direction if not currently moving), with a short invulnerability
/// window (i-frame, via Hurtbox.Invulnerable) and a cooldown so it can't
/// be spammed. This is the "base" dodge asked for in Etapa 3 — no
/// dash animation/trail, just the mechanic. Does NOT touch
/// Velocity/MoveAndSlide directly; it only sets PlayerController's
/// IsDodging/DodgeVelocity, which PlayerController reads in its own
/// _PhysicsProcess. See PlayerController's class doc for why.
/// </summary>
public partial class PlayerDodge : Node
{
    [Export] public float DodgeSpeed = 520f;
    [Export] public float DodgeDuration = 0.18f;
    [Export] public float CooldownDuration = 0.6f;

    private PlayerController _player;
    private Hurtbox _hurtbox;
    private readonly Cooldown _cooldown = new();
    private double _activeTimeRemaining;

    public override void _Ready()
    {
        _player = GetParent<PlayerController>();
        _hurtbox = GetParent().GetNode<Hurtbox>("Hurtbox");
    }

    public override void _Process(double delta)
    {
        _cooldown.Tick(delta);

        if (_player.IsDodging)
        {
            _activeTimeRemaining -= delta;
            if (_activeTimeRemaining <= 0.0)
            {
                EndDodge();
            }
            return;
        }

        if (_cooldown.IsReady && Input.IsActionJustPressed("dodge"))
        {
            StartDodge();
        }
    }

    private void StartDodge()
    {
        Vector2 moveInput = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        Vector2 direction = moveInput != Vector2.Zero ? moveInput.Normalized() : _player.FacingDirection;

        _activeTimeRemaining = DodgeDuration;
        _cooldown.Start(CooldownDuration);
        _hurtbox.Invulnerable = true;
        _player.IsDodging = true;
        _player.DodgeVelocity = direction * DodgeSpeed;
    }

    private void EndDodge()
    {
        _player.IsDodging = false;
        _hurtbox.Invulnerable = false;
    }
}
