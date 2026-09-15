using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Game.Narrative;

namespace Game.Quests;

/// <summary>
/// Autoload ou static manager para rastrear missões ativas e carregá-las de um arquivo JSON.
/// </summary>
public static class QuestManager
{
    private static Dictionary<string, QuestDefinition> _database = new();
    
    // Lista das IDs das quests aceitas
    private static List<string> _activeQuests = new();
    
    // Estado dos objetivos: chave = "QuestId_ObjectiveId", valor = quantidade atual
    private static Dictionary<string, int> _objectiveProgress = new();

    public static IReadOnlyList<string> ActiveQuests => _activeQuests;

    static QuestManager()
    {
        LoadDatabase();
    }

    private static void LoadDatabase()
    {
        using var file = FileAccess.Open("res://Data/Quests/quests.json", FileAccess.ModeFlags.Read);
        if (file == null)
        {
            GD.PrintErr($"Failed to open quests.json: {FileAccess.GetOpenError()}");
            return;
        }

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        try
        {
            var list = JsonSerializer.Deserialize<List<QuestDefinition>>(file.GetAsText(), options);
            if (list != null)
            {
                foreach (var q in list)
                {
                    _database[q.Id] = q;
                }
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Error parsing quests.json: {ex.Message}");
        }
    }

    public static QuestDefinition GetQuest(string id)
    {
        return _database.TryGetValue(id, out var quest) ? quest : null;
    }

    public static void AcceptQuest(string id)
    {
        if (!_activeQuests.Contains(id) && _database.ContainsKey(id))
        {
            _activeQuests.Add(id);
            GD.Print($"Quest Accepted: {id}");
        }
    }

    public static void ReportObjectiveProgress(string objectiveId, int amount = 1)
    {
        foreach (var questId in _activeQuests.ToList())
        {
            var quest = _database[questId];
            foreach (var obj in quest.Objectives)
            {
                if (obj.Id == objectiveId)
                {
                    string key = $"{questId}_{objectiveId}";
                    if (!_objectiveProgress.ContainsKey(key))
                        _objectiveProgress[key] = 0;
                        
                    _objectiveProgress[key] += amount;
                    GD.Print($"Objective Updated: {key} -> {_objectiveProgress[key]}/{obj.TargetAmount}");
                    
                    CheckQuestCompletion(questId);
                }
            }
        }
    }

    public static int GetObjectiveProgress(string questId, string objectiveId)
    {
        return _objectiveProgress.TryGetValue($"{questId}_{objectiveId}", out int val) ? val : 0;
    }

    private static void CheckQuestCompletion(string questId)
    {
        var quest = _database[questId];
        bool allCompleted = true;
        foreach (var obj in quest.Objectives)
        {
            if (GetObjectiveProgress(questId, obj.Id) < obj.TargetAmount)
            {
                allCompleted = false;
                break;
            }
        }

        if (allCompleted)
        {
            GD.Print($"Quest Completed: {questId}!");
            _activeQuests.Remove(questId);
            WorldState.Set($"quest_completed_{questId}", 1);
            
            // Apply rewards
            foreach (var reward in quest.Rewards)
            {
                if (reward.Type == "affinity")
                {
                    RelationshipManager.AddAffinity(reward.Id, reward.Amount);
                }
                // future: add items to inventory
            }
        }
    }
}
