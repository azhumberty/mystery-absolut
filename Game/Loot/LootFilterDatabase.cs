using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;
using Game.Items;

namespace Game.Loot;

/// <summary>
/// Loads <c>res://Data/Loot/loot_filter.json</c> once (same lazy
/// singleton + <see cref="Godot.FileAccess"/> + <c>System.Text.Json</c>
/// pattern as <see cref="ItemDatabase"/>/<see cref="AffixDatabase"/>) and
/// evaluates it against a dropped item: first matching
/// <see cref="LootFilterRule"/> (top-to-bottom) wins; if none match, the
/// result is exactly the Etapa 11 rarity default (always shown). A
/// missing or empty filter file is not an error — it just means "no
/// custom rules, everything uses the Etapa 11 defaults", so the game
/// still works with zero configuration.
/// </summary>
public sealed class LootFilterDatabase
{
    private const string FilterPath = "res://Data/Loot/loot_filter.json";
    private const int DefaultFontSize = 10;

    private static LootFilterDatabase _instance;
    public static LootFilterDatabase Instance => _instance ??= new LootFilterDatabase();

    private readonly List<LootFilterRule> _rules = new();

    private LootFilterDatabase()
    {
        Load();
    }

    public LootFilterResult Evaluate(ItemInstance instance, ItemBaseDefinition definition)
    {
        LootPresentation.Style fallback = LootPresentation.GetStyle(instance.Rarity);

        foreach (LootFilterRule rule in _rules)
        {
            if (rule.Matches(instance, definition))
            {
                return Resolve(rule, fallback);
            }
        }

        return new LootFilterResult(
            show: true,
            fontColor: fallback.Color,
            fontSize: DefaultFontSize,
            showOutline: fallback.ShowOutline,
            outlineColor: fallback.Color,
            showGlow: fallback.ShowGlow,
            glowColor: fallback.Color,
            showBeam: fallback.ShowBeam,
            beamColor: fallback.Color,
            showParticles: fallback.ShowParticles);
    }

    private static LootFilterResult Resolve(LootFilterRule rule, LootPresentation.Style fallback)
    {
        return new LootFilterResult(
            show: rule.Show,
            fontColor: rule.FontColor?.ToColor() ?? fallback.Color,
            fontSize: rule.FontSize ?? DefaultFontSize,
            showOutline: rule.ShowOutline ?? fallback.ShowOutline,
            outlineColor: rule.OutlineColor?.ToColor() ?? fallback.Color,
            showGlow: rule.ShowGlow ?? fallback.ShowGlow,
            glowColor: rule.GlowColor?.ToColor() ?? fallback.Color,
            showBeam: rule.ShowBeam ?? fallback.ShowBeam,
            beamColor: rule.BeamColor?.ToColor() ?? fallback.Color,
            showParticles: rule.ShowParticles ?? fallback.ShowParticles);
    }

    private void Load()
    {
        if (!Godot.FileAccess.FileExists(FilterPath))
        {
            GD.Print($"LootFilterDatabase: '{FilterPath}' not found — no custom rules, using Etapa 11 defaults for everything.");
            return;
        }

        using var file = Godot.FileAccess.Open(FilterPath, Godot.FileAccess.ModeFlags.Read);
        if (file == null)
        {
            GD.PushError($"LootFilterDatabase: could not open '{FilterPath}' (error {Godot.FileAccess.GetOpenError()}).");
            return;
        }

        string json = file.GetAsText();
        List<LootFilterRule> entries;
        try
        {
            entries = JsonSerializer.Deserialize<List<LootFilterRule>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() },
            });
        }
        catch (JsonException exception)
        {
            GD.PushError($"LootFilterDatabase: failed to parse '{FilterPath}': {exception.Message}");
            return;
        }

        if (entries != null)
        {
            _rules.AddRange(entries);
        }
    }
}
