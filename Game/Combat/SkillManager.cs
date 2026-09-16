using Godot;
using Game.Core;

namespace Game.Combat;

public partial class SkillManager : Node
{
    [Export] public PackedScene FireballScene;
    [Export] public float FireballManaCost = 20f;
    [Export] public float FireballCooldown = 1.0f;

    private float _fireballTimer = 0f;
    private Mana _mana;

    public override void _Ready()
    {
        _mana = GetParent().GetNodeOrNull<Mana>("Mana");
    }

    public override void _Process(double delta)
    {
        if (_fireballTimer > 0f) _fireballTimer -= (float)delta;
    }

    public bool CastFireball(Vector2 direction, Node2D owner)
    {
        if (_fireballTimer > 0f) return false;
        if (_mana == null || !_mana.TryConsume(FireballManaCost)) return false;

        _fireballTimer = FireballCooldown;

        if (FireballScene != null)
        {
            var proj = FireballScene.Instantiate<Projectile>();
            proj.Direction = direction;
            proj.Owner2D = owner;
            proj.GlobalPosition = owner.GlobalPosition;
            
            // Add to scene root (don't attach to player so it flies independently)
            owner.GetTree().CurrentScene.AddChild(proj);

            var audio = owner.GetNodeOrNull<AudioManager>("/root/AudioManager");
            audio?.PlayMagic();

            return true;
        }

        return false;
    }
}
