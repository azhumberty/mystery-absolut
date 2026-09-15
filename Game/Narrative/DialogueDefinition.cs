using System.Collections.Generic;

namespace Game.Narrative;

public class DialogueDefinition
{
    public string Id { get; set; } = string.Empty;
    public string StartNodeId { get; set; } = "start";
    
    // Dicionário de nós indexados por ID
    public Dictionary<string, DialogueNode> Nodes { get; set; } = new();
}
