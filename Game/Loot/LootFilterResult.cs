using Godot;

namespace Game.Loot;

/// <summary>
/// The fully-resolved "how should this specific ground item look/behave
/// right now" — output of <see cref="LootFilterDatabase.Evaluate"/>,
/// consumed by <see cref="GroundItem.ApplyPresentation"/>. Every field is
/// already resolved (rule action if the matching rule set it, otherwise
/// the Etapa 11 rarity default) — <c>GroundItem</c> never needs to know
/// about rules or fallbacks, just these final values.
/// </summary>
public readonly struct LootFilterResult
{
    public bool Show { get; }
    public Color FontColor { get; }
    public int FontSize { get; }
    public bool ShowOutline { get; }
    public Color OutlineColor { get; }
    public bool ShowGlow { get; }
    public Color GlowColor { get; }
    public bool ShowBeam { get; }
    public Color BeamColor { get; }
    public bool ShowParticles { get; }

    public LootFilterResult(
        bool show,
        Color fontColor,
        int fontSize,
        bool showOutline,
        Color outlineColor,
        bool showGlow,
        Color glowColor,
        bool showBeam,
        Color beamColor,
        bool showParticles)
    {
        Show = show;
        FontColor = fontColor;
        FontSize = fontSize;
        ShowOutline = showOutline;
        OutlineColor = outlineColor;
        ShowGlow = showGlow;
        GlowColor = glowColor;
        ShowBeam = showBeam;
        BeamColor = beamColor;
        ShowParticles = showParticles;
    }
}
