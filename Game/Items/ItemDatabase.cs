using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;

namespace Game.Items;

/// <summary>
/// Loads <c>res://Data/Items/items.json</c> once and exposes every
/// <see cref="ItemBaseDefinition"/> by <see cref="ItemBaseDefinition.Id"/>.
/// Plain lazy static singleton, not an autoload Node — nothing here needs
/// <c>_Ready()</c>/<c>_Process()</c> or scene-tree access, so a Node (and
/// the extra project.godot autoload entry) would be unnecessary ceremony.
/// First access from anywhere (typically <see cref="ItemInstance.GetBase"/>
/// or <c>LootGenerator</c>) triggers the load.
///
/// Uses <see cref="Godot.FileAccess"/> (not System.IO) specifically because
/// this file lives under res://, which is a real OS path only inside the
/// editor/debug build — in an exported build it's packed into the .pck and
/// only Godot's own file API can read it.
/// </summary>
public sealed class ItemDatabase
{
    private const string DatabasePath = "res://Data/Items/items.json";

    private static ItemDatabase _instance;
    public static ItemDatabase Instance => _instance ??= new ItemDatabase();

    private readonly Dictionary<string, ItemBaseDefinition> _byId = new();

    private ItemDatabase()
    {
        Load();
    }

    public ItemBaseDefinition Get(string id)
    {
        return _byId.TryGetValue(id, out ItemBaseDefinition definition) ? definition : null;
    }

    public bool TryGet(string id, out ItemBaseDefinition definition) => _byId.TryGetValue(id, out definition);

    public IReadOnlyCollection<ItemBaseDefinition> GetAll() => _byId.Values;

    private void Load()
    {
        using var file = Godot.FileAccess.Open(DatabasePath, Godot.FileAccess.ModeFlags.Read);
        if (file == null)
        {
            GD.PushError($"ItemDatabase: could not open '{DatabasePath}' (error {Godot.FileAccess.GetOpenError()}).");
            return;
        }

        string json = file.GetAsText();
        List<ItemBaseDefinition> entries;
        try
        {
            entries = JsonSerializer.Deserialize<List<ItemBaseDefinition>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() },
            });
        }
        catch (JsonException exception)
        {
            GD.PushError($"ItemDatabase: failed to parse '{DatabasePath}': {exception.Message}");
            return;
        }

        if (entries == null)
        {
            return;
        }

        foreach (ItemBaseDefinition entry in entries)
        {
            if (string.IsNullOrEmpty(entry.Id))
            {
                GD.PushWarning("ItemDatabase: skipping entry with empty id.");
                continue;
            }

            if (!_byId.TryAdd(entry.Id, entry))
            {
                GD.PushWarning($"ItemDatabase: duplicate item id '{entry.Id}', keeping the first one.");
            }
        }
    }
}
