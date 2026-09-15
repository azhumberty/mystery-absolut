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

        // Etapa 2 (Combat Foundation) + Etapa 3 (Dodge/Block). Key choices
        // are placeholders (arbitrary, easily remapped later) — picked
        // plain letter/space keys deliberately, not modifier keys like
        // Shift/Ctrl, to avoid any ambiguity about whether Godot's Key
        // enum exposes a bare "generic modifier" keycode.
        RegisterAction("attack", Key.Space);
        RegisterAction("dodge", Key.C);
        RegisterAction("block", Key.X);

        // Etapa 7 (Inventory): toggles the basic inventory panel
        // (UI/InventoryUI.cs). Same reasoning as attack/dodge/block — a
        // plain letter key, not a modifier.
        RegisterAction("inventory", Key.I);

        // Etapa 13 (Stash): toggles the stash panel (UI/StashUI.cs),
        // independent of "inventory" so both can be open at once.
        RegisterAction("stash", Key.T);

        // Etapa 15 (Companion AI Avançada): Regroup
        RegisterAction("companion_regroup", Key.V);

        // Reserved for upcoming stages. Intentionally NOT bound to any key
        // yet — declaring the action here only reserves the name so future
        // systems can reference it without touching this file.
        // RegisterAction("skill_1");
        
        // Etapa 16: Interact
        RegisterAction("interact", Key.E);
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
