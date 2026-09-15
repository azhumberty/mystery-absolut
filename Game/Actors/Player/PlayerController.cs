using Godot;

namespace Game.Actors.Player;

/// <summary>
/// Owns only input, intention and movement for the player character, per
/// MASTER_HANDOFF section 44/67: no HP, inventory, loot, quests, crafting
/// or dialogue logic belongs here. Those will be added later as separate,
/// composed systems (Etapa 2+).
/// </summary>
public partial class PlayerController : CharacterBody2D
{
    [Export] public float MoveSpeed = 220f;

    public override void _PhysicsProcess(double delta)
    {
        Velocity = GetMovementInput() * MoveSpeed;
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
