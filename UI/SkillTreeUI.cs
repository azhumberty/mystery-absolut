using Godot;
using Game.Combat;

namespace Game.UI;

public partial class SkillTreeUI : CanvasLayer
{
    private Panel _panel;
    private Label _pointsLabel;
    private Stats _stats;

    public override void _Ready()
    {
        _panel = new Panel
        {
            CustomMinimumSize = new Vector2(400, 300),
            Position = new Vector2(400, 200),
            Visible = false
        };
        AddChild(_panel);

        var vbox = new VBoxContainer { CustomMinimumSize = new Vector2(380, 280), Position = new Vector2(10, 10) };
        _panel.AddChild(vbox);

        _pointsLabel = new Label { Text = "Skill Points: 0" };
        vbox.AddChild(_pointsLabel);

        vbox.AddChild(new HSeparator());

        var btnStr = new Button { Text = "Increase Strength (+1)" };
        btnStr.Pressed += () => SpendPoint("STR");
        vbox.AddChild(btnStr);

        var btnDex = new Button { Text = "Increase Dexterity (+1)" };
        btnDex.Pressed += () => SpendPoint("DEX");
        vbox.AddChild(btnDex);

        var btnInt = new Button { Text = "Increase Intelligence (+1)" };
        btnInt.Pressed += () => SpendPoint("INT");
        vbox.AddChild(btnInt);

        // Bind to player asynchronously
        Callable.From(() => ConnectToPlayer()).CallDeferred();
    }

    private void ConnectToPlayer()
    {
        var player = GetTree().GetFirstNodeInGroup("player");
        if (player != null)
        {
            _stats = player.GetNodeOrNull<Stats>("Stats");
            if (_stats != null)
            {
                _stats.LevelUp += (int level) => UpdateUI();
                UpdateUI();
            }
        }
    }

    private void UpdateUI()
    {
        if (_stats != null)
        {
            _pointsLabel.Text = $"Level: {_stats.Level} | Skill Points: {_stats.SkillPoints}\nSTR: {_stats.Strength} | DEX: {_stats.Dexterity} | INT: {_stats.Intelligence}";
        }
    }

    private void SpendPoint(string stat)
    {
        if (_stats != null && _stats.SkillPoints > 0)
        {
            _stats.SkillPoints--;
            switch (stat)
            {
                case "STR": _stats.Strength++; break;
                case "DEX": _stats.Dexterity++; break;
                case "INT": _stats.Intelligence++; break;
            }
            UpdateUI();
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo && keyEvent.Keycode == Key.K)
        {
            _panel.Visible = !_panel.Visible;
            if (_panel.Visible) UpdateUI();
        }
    }
}
