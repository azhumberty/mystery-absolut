namespace Game.Narrative;

/// <summary>
/// Wrapper sobre o WorldState para manipular afinidade e memórias de NPCs de forma mais legível.
/// </summary>
public static class RelationshipManager
{
    public static int GetAffinity(string npcId)
    {
        return WorldState.Get($"affinity_{npcId}");
    }

    public static void AddAffinity(string npcId, int amount)
    {
        WorldState.Increment($"affinity_{npcId}", amount);
    }

    public static void AddMemory(string npcId, string memoryId)
    {
        WorldState.Set($"memory_{npcId}_{memoryId}", 1);
    }

    public static bool HasMemory(string npcId, string memoryId)
    {
        return WorldState.Get($"memory_{npcId}_{memoryId}") > 0;
    }
}
