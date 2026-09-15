using System.Collections.Generic;
using Game.Items;

namespace Game.Loot;

/// <summary>
/// One rule of the Loot Filter (Etapa 12 — MASTER_HANDOFF seção 28-36,
/// "inspirado no FilterBlade: rules com conditions... e actions..."),
/// loaded from <c>Data/Loot/loot_filter.json</c> by
/// <see cref="LootFilterDatabase"/>. Rules are evaluated top-to-bottom;
/// the first one whose conditions all match wins (same convention as
/// FilterBlade/real ARPG loot filters) — see
/// <see cref="LootFilterDatabase.Evaluate"/>.
///
/// **Conditions implemented, matching what the game actually has data
/// for right now**: tags, rarity, item level range, "is this a crafting
/// currency". **Deliberately NOT implemented** (documented, not silently
/// missing): base type / class / slot / affixes / tiers / quantity /
/// quest item / sockets — MASTER_HANDOFF seção 28-36 lists these as
/// eventual FilterBlade-parity conditions, but "class" and "sockets"
/// don't exist as systems yet, "slot" would duplicate
/// EquipmentSlotCategory matching that no rule needs yet, and per-affix
/// conditions need a small query language of their own — none of that is
/// worth inventing speculatively for a filter that currently has to work
/// with 10 item bases and 8 affixes total. Extending
/// <see cref="Matches"/> later with more conditions is additive and safe.
///
/// **Actions implemented**: Show/Hide, FontColor, FontSize, and
/// enable+color for Outline/Glow/Beam/Particles. **Deliberately NOT
/// implemented**: Sound/SoundVolume (no audio assets exist in this repo
/// — see <see cref="LootPresentation"/> doc) and MinimapIcon/MinimapColor
/// (there is no minimap system at all yet — Etapa 28-36 lists it as a
/// future concern, not something to invent a placeholder for here).
/// </summary>
public class LootFilterRule
{
    // --- Conditions (null/empty = "don't care", matches anything) ---
    public List<string> Tags { get; set; }
    public List<ItemRarity> RarityIn { get; set; }
    public int? MinItemLevel { get; set; }
    public int? MaxItemLevel { get; set; }
    public bool? IsCurrency { get; set; }

    // --- Actions ---
    public bool Show { get; set; } = true;
    public LootFilterColor FontColor { get; set; }
    public int? FontSize { get; set; }
    public bool? ShowOutline { get; set; }
    public LootFilterColor OutlineColor { get; set; }
    public bool? ShowGlow { get; set; }
    public LootFilterColor GlowColor { get; set; }
    public bool? ShowBeam { get; set; }
    public LootFilterColor BeamColor { get; set; }
    public bool? ShowParticles { get; set; }

    /// <summary>True only if every condition set on this rule matches the given item.</summary>
    public bool Matches(ItemInstance instance, ItemBaseDefinition definition)
    {
        if (Tags != null && Tags.Count > 0)
        {
            bool anyTagMatches = false;
            foreach (string tag in Tags)
            {
                if (definition != null && definition.HasTag(tag))
                {
                    anyTagMatches = true;
                    break;
                }
            }

            if (!anyTagMatches)
            {
                return false;
            }
        }

        if (RarityIn != null && RarityIn.Count > 0 && !RarityIn.Contains(instance.Rarity))
        {
            return false;
        }

        if (MinItemLevel.HasValue && instance.ItemLevel < MinItemLevel.Value)
        {
            return false;
        }

        if (MaxItemLevel.HasValue && instance.ItemLevel > MaxItemLevel.Value)
        {
            return false;
        }

        if (IsCurrency.HasValue && definition != null && definition.IsCraftingCurrency != IsCurrency.Value)
        {
            return false;
        }

        return true;
    }
}
