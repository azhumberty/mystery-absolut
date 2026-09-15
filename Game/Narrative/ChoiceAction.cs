namespace Game.Narrative;

/// <summary>
/// Representa uma ação executada quando uma escolha é feita (ex: mudar o mundo).
/// </summary>
public class ChoiceAction
{
    public string Key { get; set; } = string.Empty;
    public string Operator { get; set; } = "="; // =, +=, -=
    public int Value { get; set; }

    public void Execute()
    {
        if (string.IsNullOrEmpty(Key)) return;

        switch (Operator)
        {
            case "=":
                WorldState.Set(Key, Value);
                break;
            case "+=":
                WorldState.Increment(Key, Value);
                break;
            case "-=":
                WorldState.Increment(Key, -Value);
                break;
        }
    }
}
