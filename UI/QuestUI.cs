using Godot;
using Game.Quests;

namespace Game.UI;

public partial class QuestUI : CanvasLayer
{
    private Panel _panel;
    private VBoxContainer _questListContainer;

    public override void _Ready()
    {
        _panel = new Panel
        {
            CustomMinimumSize = new Vector2(400, 300),
            Position = new Vector2(300, 150),
            Visible = false
        };
        AddChild(_panel);

        var margin = new MarginContainer();
        margin.AddThemeConstantOverride("margin_left", 20);
        margin.AddThemeConstantOverride("margin_top", 20);
        margin.AddThemeConstantOverride("margin_right", 20);
        margin.AddThemeConstantOverride("margin_bottom", 20);
        margin.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        _panel.AddChild(margin);

        var vbox = new VBoxContainer();
        margin.AddChild(vbox);

        var title = new Label
        {
            Text = "Diário de Missões",
            HorizontalAlignment = HorizontalAlignment.Center
        };
        vbox.AddChild(title);

        _questListContainer = new VBoxContainer();
        vbox.AddChild(_questListContainer);
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("quest_log"))
        {
            _panel.Visible = !_panel.Visible;
            if (_panel.Visible)
            {
                RefreshUI();
            }
        }
    }

    private void RefreshUI()
    {
        foreach (Node child in _questListContainer.GetChildren())
        {
            child.QueueFree();
        }

        var activeQuests = QuestManager.ActiveQuests;
        if (activeQuests.Count == 0)
        {
            _questListContainer.AddChild(new Label { Text = "Nenhuma missão ativa." });
            return;
        }

        foreach (var qId in activeQuests)
        {
            var quest = QuestManager.GetQuest(qId);
            if (quest == null) continue;

            var qTitle = new Label { Text = $"- {quest.Name}" };
            qTitle.AddThemeColorOverride("font_color", new Color(1, 0.8f, 0.2f));
            _questListContainer.AddChild(qTitle);

            foreach (var obj in quest.Objectives)
            {
                int current = QuestManager.GetObjectiveProgress(quest.Id, obj.Id);
                var objLabel = new Label { Text = $"  {obj.Description}: {current}/{obj.TargetAmount}" };
                _questListContainer.AddChild(objLabel);
            }
        }
    }
}
