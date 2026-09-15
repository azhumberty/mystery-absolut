using System;
using System.Collections.Generic;

namespace Game.Items;

/// <summary>
/// One concrete, ownable item — dropped on the ground, sitting in an
/// Inventory slot, or (Etapa 8+) equipped. Points at a shared
/// <see cref="ItemBaseDefinition"/> by <see cref="BaseId"/> instead of
/// copying its data, so changing a base in <c>items.json</c> doesn't
/// require touching every instance of it.
///
/// No affix list yet (Etapa 9) — <see cref="Rarity"/> and
/// <see cref="ItemLevel"/> exist now because Etapa 6 explicitly asks for
/// them, but nothing reads them to roll modifiers until Affixes exists.
/// </summary>
public class ItemInstance
{
    public string InstanceId { get; }
    public string BaseId { get; }
    public ItemRarity Rarity { get; set; }

    /// <summary>
    /// Level the item "rolled" at — will limit which affixes can appear on
    /// it once Etapa 9 (Affixes) exists. No enemy/area level system exists
    /// yet, so callers currently pass a placeholder (see
    /// <c>LootGenerator</c>) — this is NOT a real progression value yet,
    /// just the field the future system will read.
    /// </summary>
    public int ItemLevel { get; set; }

    /// <summary>
    /// How many units this single inventory slot represents. Only
    /// meaningful when the underlying base is
    /// <see cref="ItemBaseDefinition.Stackable"/>; non-stackable items
    /// (equipment) always carry 1.
    /// </summary>
    public int StackCount { get; set; }

    /// <summary>
    /// Rolled affixes (Etapa 9) — empty for Normal-rarity and stackable
    /// (currency/consumable) items. See <see cref="AffixRoller"/> for how
    /// these get populated/changed.
    /// </summary>
    public List<AffixInstance> Affixes { get; set; } = new();

    public ItemInstance(string baseId, ItemRarity rarity, int itemLevel, int stackCount = 1)
    {
        InstanceId = Guid.NewGuid().ToString("N");
        BaseId = baseId;
        Rarity = rarity;
        ItemLevel = itemLevel;
        StackCount = Math.Max(1, stackCount);
    }

    public ItemBaseDefinition GetBase() => ItemDatabase.Instance.Get(BaseId);
}
