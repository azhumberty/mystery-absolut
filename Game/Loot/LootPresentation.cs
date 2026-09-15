using System.Collections.Generic;
using Godot;
using Game.Items;

namespace Game.Loot;

/// <summary>
/// Maps <see cref="ItemRarity"/> to "how should this look sitting on the
/// ground" (Etapa 11 — MASTER_HANDOFF seção 28-36: "label, cor, borda,
/// glow, partículas, beam, som... Rare com visual moderado, Unique com
/// visual grande"). <see cref="GroundItem"/> reads this once in
/// <c>_Ready()</c> and builds its presentation child nodes from it.
///
/// **Deliberately code, not JSON**, unlike items/affixes: there are only
/// 4 fixed rarities, and these are art-direction/placeholder constants
/// (closer in spirit to <c>AffixRoller.MaxAffixesFor</c>'s hardcoded caps
/// than to real content an end user adds entries to). Etapa 12 (Loot
/// Filter) is the actual data-driven layer on top of this — per-rule
/// overrides loaded from JSON that can override what's here.
///
/// **Sound is deliberately not part of this etapa**, documented as a
/// non-goal rather than a placeholder: there are no audio assets in this
/// repository yet and nothing to safely author/import without the Godot
/// editor open to verify it. Adding an `AudioStreamPlayer2D` with no
/// `Stream` assigned would just be dead code. When real SFX exist, the
/// natural hook is inside <see cref="GroundItem.ApplyPresentation"/>,
/// next to where the other rarity-based effects are built.
/// </summary>
public static class LootPresentation
{
    /// <summary>One resolved "look" for a ground item — consumed by <see cref="GroundItem"/>.</summary>
    public readonly struct Style
    {
        public Color Color { get; }
        public bool ShowOutline { get; }
        public bool ShowGlow { get; }
        public bool ShowBeam { get; }
        public bool ShowParticles { get; }

        public Style(Color color, bool showOutline, bool showGlow, bool showBeam, bool showParticles)
        {
            Color = color;
            ShowOutline = showOutline;
            ShowGlow = showGlow;
            ShowBeam = showBeam;
            ShowParticles = showParticles;
        }
    }

    private static readonly Dictionary<ItemRarity, Style> Styles = new()
    {
        // Normal: só a cor base (cinza claro) + label. Sem nenhum efeito extra.
        { ItemRarity.Normal, new Style(new Color(0.85f, 0.85f, 0.85f), false, false, false, false) },

        // Magic: azul, com contorno (outline) pra destacar da Normal à distância.
        // Sem glow/beam/partículas — a diferenciação visual de Magic é sutil por design.
        { ItemRarity.Magic, new Style(new Color(0.35f, 0.55f, 1f), true, false, false, false) },

        // Rare: dourado. "Visual moderado": contorno + glow pulsante + beam curto.
        // Sem partículas (isso fica só pra Unique, ver classe doc).
        { ItemRarity.Rare, new Style(new Color(1f, 0.85f, 0.2f), true, true, true, false) },

        // Unique: laranja. "Visual grande": tudo, incluindo partículas.
        // Ainda não é sorteável pelo LootGenerator (Unique fica de fora do roll —
        // ver doc em LootGenerator.RollRarity), mas o estilo já existe pronto pra
        // quando esse sistema existir, sem precisar mexer aqui de novo.
        { ItemRarity.Unique, new Style(new Color(1f, 0.5f, 0.1f), true, true, true, true) },
    };

    public static Style GetStyle(ItemRarity rarity)
    {
        return Styles.TryGetValue(rarity, out Style style) ? style : Styles[ItemRarity.Normal];
    }
}
