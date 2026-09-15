using Godot;
using Game.UI;

namespace Game.World;

public partial class TestWorld : Node2D
{
    private DialogueUI _dialogueUI;

    public override void _Ready()
    {
        _dialogueUI = GetNodeOrNull<DialogueUI>("DialogueUI");
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("interact") && _dialogueUI != null)
        {
            _dialogueUI.StartDialogue("test_companion_talk");
        }
    }
}
