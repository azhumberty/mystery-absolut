using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Game.Narrative;

public static class DialogueDatabase
{
    private static Dictionary<string, DialogueDefinition> _dialogues;

    public static IReadOnlyDictionary<string, DialogueDefinition> Dialogues
    {
        get
        {
            if (_dialogues == null)
            {
                LoadData();
            }
            return _dialogues;
        }
    }

    public static DialogueDefinition GetDialogue(string id)
    {
        return Dialogues.TryGetValue(id, out var dialogue) ? dialogue : null;
    }

    private static void LoadData()
    {
        _dialogues = new Dictionary<string, DialogueDefinition>();

        using var file = FileAccess.Open("res://Data/Narrative/dialogues.json", FileAccess.ModeFlags.Read);
        if (file == null)
        {
            GD.PrintErr($"Failed to open dialogues.json: {FileAccess.GetOpenError()}");
            return;
        }

        string jsonString = file.GetAsText();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        
        try
        {
            var list = JsonSerializer.Deserialize<List<DialogueDefinition>>(jsonString, options);
            if (list != null)
            {
                foreach (var dialogue in list)
                {
                    _dialogues[dialogue.Id] = dialogue;
                }
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Error parsing dialogues.json: {ex.Message}");
        }
    }
}
