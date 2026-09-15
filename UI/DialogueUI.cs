using Godot;
using Game.Narrative;
using System.Collections.Generic;

namespace Game.UI;

public partial class DialogueUI : CanvasLayer
{
    private Panel _panel;
    private Label _speakerLabel;
    private Label _textLabel;
    private VBoxContainer _choicesContainer;

    private DialogueDefinition _currentDialogue;
    private string _currentNodeId;

    public override void _Ready()
    {
        // UI Construída em código para evitar erros silenciosos de edição de tscn sem o editor Godot
        _panel = new Panel
        {
            CustomMinimumSize = new Vector2(600, 200),
            Position = new Vector2(100, 400),
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

        _speakerLabel = new Label
        {
            AutowrapMode = TextServer.AutowrapMode.Word,
            Visible = false
        };
        _speakerLabel.AddThemeColorOverride("font_color", new Color(1, 0.8f, 0.2f));
        vbox.AddChild(_speakerLabel);

        _textLabel = new Label
        {
            AutowrapMode = TextServer.AutowrapMode.Word,
            CustomMinimumSize = new Vector2(560, 60)
        };
        vbox.AddChild(_textLabel);

        _choicesContainer = new VBoxContainer();
        vbox.AddChild(_choicesContainer);
    }

    public void StartDialogue(string dialogueId)
    {
        var dialogue = DialogueDatabase.GetDialogue(dialogueId);
        if (dialogue == null)
        {
            GD.PrintErr($"Dialogue {dialogueId} not found");
            return;
        }

        _currentDialogue = dialogue;
        
        // Verifica condição externa no start node se necessário
        // Para simplificar, começamos no StartNodeId normal ou adaptamos via código (Etapa 17)
        string startNode = _currentDialogue.StartNodeId;
        
        // Exemplo: se já recrutou o companion, vai pra outro nó. (Na vida real, a chamada de StartDialogue decidiria o nó inicial)
        if (dialogueId == "test_companion_talk" && WorldState.Get("companion_hired") == 1)
        {
            startNode = "already_hired";
        }

        ShowNode(startNode);
        _panel.Visible = true;
        GetTree().Paused = true; // Pausa o jogo durante o diálogo
    }

    private void ShowNode(string nodeId)
    {
        if (string.IsNullOrEmpty(nodeId) || !_currentDialogue.Nodes.TryGetValue(nodeId, out var node))
        {
            CloseDialogue();
            return;
        }

        _currentNodeId = nodeId;

        if (!string.IsNullOrEmpty(node.Speaker))
        {
            _speakerLabel.Text = node.Speaker + ":";
            _speakerLabel.Visible = true;
        }
        else
        {
            _speakerLabel.Visible = false;
        }

        _textLabel.Text = node.Text;

        // Limpa opções antigas
        foreach (Node child in _choicesContainer.GetChildren())
        {
            child.QueueFree();
        }

        // Cria novos botões para as escolhas
        foreach (var choice in node.Choices)
        {
            // Checa condição
            if (choice.Condition != null && !choice.Condition.IsMet())
            {
                continue; // Condição não atendida, pula essa escolha
            }

            var btn = new Button { Text = choice.Text };
            btn.Pressed += () => OnChoiceSelected(choice);
            _choicesContainer.AddChild(btn);
        }
    }

    private void OnChoiceSelected(DialogueChoice choice)
    {
        if (choice.Action != null)
        {
            choice.Action.Execute();
        }

        ShowNode(choice.NextNodeId);
    }

    private void CloseDialogue()
    {
        _panel.Visible = false;
        _currentDialogue = null;
        _currentNodeId = string.Empty;
        GetTree().Paused = false;
    }
}
