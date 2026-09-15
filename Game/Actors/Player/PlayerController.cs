using Godot;

namespace Game.Actors.Player;

/// <summary>
/// Owns only input, intention and movement for the player character, per
/// MASTER_HANDOFF section 44/67: no HP, inventory, loot, quests, crafting
/// or dialogue logic belongs here. Combat state (Health, Hurtbox, attack,
/// dodge, block) lives in sibling components under composition instead.
///
/// Movement authority stays entirely in this class, even during a dodge:
/// PlayerDodge only sets IsDodging/DodgeVelocity (intent), it never calls
/// MoveAndSlide() itself. Two scripts both driving MoveAndSlide() on the
/// same body in the same physics frame would fight each other — keeping
/// a single source of truth here avoids that class of bug entirely.
/// </summary>
public partial class PlayerController : CharacterBody2D
{
    [Export] public float MoveSpeed = 220f;

    /// <summary>
    /// Last non-zero movement input direction. Used by combat components
    /// (PlayerBasicAttack, PlayerDodge) that need a facing/aim direction,
    /// since there's no separate aiming system yet. Defaults to facing
    /// down, matching common top-down RPG conventions.
    /// </summary>
    public Vector2 FacingDirection { get; private set; } = Vector2.Down;

    /// <summary>Set by PlayerDodge while a dodge is active. See class doc.</summary>
    public bool IsDodging { get; set; }
    public Vector2 DodgeVelocity { get; set; }

    public override void _Ready()
    {
        // Lets enemies (Game.Actors.Enemies.EnemyController) and pickups
        // (Game.Loot.GroundItem) find/recognize the player without a
        // direct reference wired in the editor. Using AddToGroup here
        // instead of hand-writing `groups=[...]` in Player.tscn — I'm not
        // fully certain of that property's exact raw serialization
        // syntax, and a wrong group name in code is a compile-checked,
        // obvious mistake, while a malformed line in the .tscn text could
        // fail silently.
        AddToGroup("player");
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 inputDirection = GetMovementInput();
        if (inputDirection != Vector2.Zero)
        {
            FacingDirection = inputDirection.Normalized();
        }

        Velocity = IsDodging ? DodgeVelocity : inputDirection * MoveSpeed;
        MoveAndSlide();
    }

    /// <summary>
    /// Input.GetVector already normalizes the resulting vector's length to
    /// at most 1, so diagonal movement is not faster than cardinal
    /// movement — satisfies the 8-direction, normalized-diagonal
    /// requirement without extra manual math.
    /// </summary>
    private static Vector2 GetMovementInput()
    {
        return Input.GetVector("move_left", "move_right", "move_up", "move_down");
    }
}
