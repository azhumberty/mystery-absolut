using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;

namespace Game.Items;

/// <summary>
/// Loads <c>res://Data/Items/affixes.json</c> once and exposes every
/// <see cref="AffixDefinition"/>. Same lazy static singleton shape as
/// <see cref="ItemDatabase"/>, same reasoning (no Node/autoload needed,
/// Godot.FileAccess + System.Text.Json instead of a Resource).
/// </summary>
public sealed class AffixDatabase
{
    private const string DatabasePath = "res://Data/Items/affixes.json";

    private static AffixDatabase _instance;
    public static AffixDatabase Instance => _instance ??= new AffixDatabase();

    private readonly Dictionary<string, AffixDefinition> _byId = new();

    private AffixDatabase()
    {
        Load();
    }

    public AffixDefinition Get(string id) => _byId.TryGetValue(id, out AffixDefinition definition) ? definition : null;

    /// <summary>
    /// Every affix of <paramref name="type"/> eligible for
    /// <paramref name="definition"/> at <paramref name="itemLevel"/>: at
    /// least one shared tag, and RequiredItemLevel satisfied.
    /// </summary>
    public List<AffixDefinition> GetEligiblePool(ItemBaseDefinition definition, AffixType type, int itemLevel)
    {
        return _byId.Values
            .Where(a => a.Type == type)
            .Where(a => a.RequiredItemLevel <= itemLevel)
            .Where(a => a.RequiredTags.Count == 0 || a.RequiredTags.Any(definition.HasTag))
            .ToList();
    }

    private void Load()
    {
        using var file = Godot.FileAccess.Open(DatabasePath, Godot.FileAccess.ModeFlags.Read);
        if (file == null)
        {
            GD.PushError($"AffixDatabase: could not open '{DatabasePath}' (error {Godot.FileAccess.GetOpenError()}).");
            return;
        }

        string json = file.GetAsText();
        List<AffixDefinition> entries;
        try
        {
            entries = JsonSerializer.Deserialize<List<AffixDefinition>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() },
            });
        }
        catch (JsonException exception)
        {
            GD.PushError($"AffixDatabase: failed to parse '{DatabasePath}': {exception.Message}");
            return;
        }

        if (entries == null)
        {
            return;
        }

        foreach (AffixDefinition entry in entries)
        {
            if (string.IsNullOrEmpty(entry.Id))
            {
                GD.PushWarning("AffixDatabase: skipping entry with empty id.");
                continue;
            }

            if (!_byId.TryAdd(entry.Id, entry))
            {
                GD.PushWarning($"AffixDatabase: duplicate affix id '{entry.Id}', keeping the first one.");
            }
        }
    }
}
