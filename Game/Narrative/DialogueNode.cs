using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Game.Narrative;

public class DialogueChoice
{
    public string Text { get; set; } = string.Empty;
    public string NextNodeId { get; set; } = string.Empty; // Id do próximo nó se escolhido (vazio = fecha o diálogo)
    
    [JsonPropertyName("condition")]
    public ChoiceCondition Condition { get; set; }

    [JsonPropertyName("action")]
    public ChoiceAction Action { get; set; }
}

public class DialogueNode
{
    public string Id { get; set; } = string.Empty;
    public string Speaker { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    
    [JsonPropertyName("choices")]
    public List<DialogueChoice> Choices { get; set; } = new();
}
