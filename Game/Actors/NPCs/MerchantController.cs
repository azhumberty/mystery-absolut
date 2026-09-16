using Godot;
using Game.UI;

namespace Game.Actors.NPCs;

public partial class MerchantController : CharacterBody2D
{
    private Area2D _interactionArea;
    private ShopUI _shopUI;

    public override void _Ready()
    {
        _interactionArea = GetNode<Area2D>("InteractionArea");
    }

    public override void _Process(double delta)
    {
        if (_shopUI == null)
        {
            // Try to find it in the current scene
            _shopUI = GetTree().CurrentScene?.GetNodeOrNull<ShopUI>("ShopUI");
        }

        if (Input.IsActionJustPressed("interact") && _interactionArea.HasOverlappingBodies())
        {
            foreach (var body in _interactionArea.GetOverlappingBodies())
            {
                if (body.IsInGroup("player"))
                {
                    _shopUI?.OpenShop();
                    break;
                }
            }
        }
    }
}
