using Godot;

namespace Game.Core;

/// <summary>
/// Autoload singleton responsible for registering the game's core InputMap
/// actions in code instead of hand-edited binary-ish entries in
/// project.godot. This keeps key bindings easy to read, review and extend
/// as new actions are needed (attack, skill_1, dodge, block, interact,
/// inventory, ...) without risking malformed InputEventKey data.
/// </summary>
public partial class InputBootstrap : Node
{
    public override void _Ready()
    {
        RegisterAction("move_up", Key.W, Key.Up);
        RegisterAction("move_down", Key.S, Key.Down);
        RegisterAction("move_left", Key.A, Key.Left);
        RegisterAction("move_right", Key.D, Key.Right);

        // Reserved for upcoming stages. Intentionally NOT bound to any key
        // yet — declaring the action here only reserves the name so future
        // systems (Etapa 2+) can reference it without touching this file.
        // RegisterAction("attack");
        // RegisterAction("skill_1");
        // RegisterAction("dodge");
        // RegisterAction("block");
        // RegisterAction("interact");
        // RegisterAction("inventory");
    }

    private static void RegisterAction(string action, params Key[] keys)
    {
        if (!InputMap.HasAction(action))
        {
            InputMap.AddAction(action);
        }

        foreach (Key key in keys)
        {
            var inputEvent = new InputEventKey { Keycode = key };
            InputMap.ActionAddEvent(action, inputEvent);
        }
    }
}
