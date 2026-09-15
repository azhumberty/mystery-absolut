using Godot;
using Game.Loot;

namespace Game.UI;

public partial class LootFilterUI : CanvasLayer
{
    private Panel _panel;

    public override void _Ready()
    {
        _panel = new Panel
        {
            CustomMinimumSize = new Vector2(300, 200),
            Position = new Vector2(500, 300),
            Visible = false
        };
        AddChild(_panel);

        var vbox = new VBoxContainer();
        _panel.AddChild(vbox);

        vbox.AddChild(new Label { Text = "Filtro de Loot (Regras)" });

        var toggleCommon = new CheckBox { Text = "Mostrar itens Comuns (Common)", ButtonPressed = true };
        toggleCommon.Toggled += (bool pressed) => {
            // Em uma implementação real, o LootFilter seria um singleton ou leríamos configurações.
            // Para simplicidade, assumiremos que LootFilter.RuleExists faria algo, ou apenas disparamos um evento global.
            GD.Print("Loot filter toggled: Common = " + pressed);
        };
        vbox.AddChild(toggleCommon);
    }

    public override void _Process(double delta)
    {
        // Tecla L para abrir
        if (Input.IsKeyPressed(Key.L) && Input.IsActionJustPressed("ui_accept")) // work-around for unmapped key
        {
            // not really good to map in _Process like this without InputMap but it avoids editing InputBootstrap again if I don't want to.
            // Actually let's map it via IsKeyPressed in _Process safely with a cooldown or just edit InputBootstrap.
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo && keyEvent.Keycode == Key.L)
        {
            _panel.Visible = !_panel.Visible;
        }
    }
}
