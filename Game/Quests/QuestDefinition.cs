using System.Collections.Generic;

namespace Game.Quests;

public class QuestReward
{
    public string Type { get; set; } = string.Empty; // "item", "affinity"
    public string Id { get; set; } = string.Empty; // itemId or npcId
    public int Amount { get; set; }
}

public class QuestObjective
{
    public string Id { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int TargetAmount { get; set; } = 1;
}

public class QuestDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<QuestObjective> Objectives { get; set; } = new();
    public List<QuestReward> Rewards { get; set; } = new();
}
