using Godot;
using Game.Quests;

namespace Game.World;

public partial class Teleporter : Area2D
{
    [Export] public string TargetScenePath = "res://Game/World/Dungeon1.tscn";

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body.IsInGroup("player"))
        {
            // Opcional: auto-aceita a quest pra testar
            QuestManager.AcceptQuest("q_first_dungeon");
            
            CallDeferred(nameof(ChangeScene));
        }
    }
    
    private void ChangeScene()
    {
        GetTree().ChangeSceneToFile(TargetScenePath);
    }
}
