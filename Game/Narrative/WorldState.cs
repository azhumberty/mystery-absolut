using System.Collections.Generic;

namespace Game.Narrative;

/// <summary>
/// Singleton simples que armazena o estado global narrativo do jogo.
/// Mantém flags e contadores (ex: quests aceitas, mortes de chefes, progresso de relação).
/// No futuro, isso será salvo/carregado do disco (Etapa 22).
/// </summary>
public static class WorldState
{
    private static readonly Dictionary<string, int> _state = new();

    public static void Set(string key, int value)
    {
        _state[key] = value;
    }

    public static int Get(string key, int defaultValue = 0)
    {
        return _state.TryGetValue(key, out int value) ? value : defaultValue;
    }

    public static void Increment(string key, int amount = 1)
    {
        Set(key, Get(key) + amount);
    }

    public static Dictionary<string, int> GetAll()
    {
        return new Dictionary<string, int>(_state);
    }

    public static void SetAll(Dictionary<string, int> data)
    {
        _state.Clear();
        foreach (var kvp in data)
        {
            _state[kvp.Key] = kvp.Value;
        }
    }

    public static void Clear()
    {
        _state.Clear();
    }
}
