namespace Game.Narrative;

/// <summary>
/// Representa uma condição para uma escolha estar disponível em um diálogo.
/// </summary>
public class ChoiceCondition
{
    public string Key { get; set; } = string.Empty;
    public string Operator { get; set; } = "=="; // ==, >, <, >=, <=, !=
    public int Value { get; set; }

    public bool IsMet()
    {
        if (string.IsNullOrEmpty(Key)) return true; // Sem condição = sempre válido

        int currentValue = WorldState.Get(Key);

        return Operator switch
        {
            "==" => currentValue == Value,
            "!=" => currentValue != Value,
            ">" => currentValue > Value,
            ">=" => currentValue >= Value,
            "<" => currentValue < Value,
            "<=" => currentValue <= Value,
            _ => false
        };
    }
}
