using System.Collections.Generic;

namespace Game.Items;

/// <summary>
/// The *definition* of a kind of item (ex: "espada curta enferrujada") —
/// shared, read-only data loaded once from <c>Data/Items/items.json</c> by
/// <see cref="ItemDatabase"/>. This is the "ItemBase" half of the
/// ItemBase → ItemInstance → Rarity → Affixes → Crafting pipeline
/// (MASTER_HANDOFF seção 18-24). Not a Godot Resource on purpose: a custom
/// Resource would need hand-authored <c>.tres</c>/<c>sub_resource</c>
/// blocks I can't validate without the Godot editor open (same reasoning
/// already used for DamageInfo/DropTable) — plain JSON, parsed with
/// System.Text.Json, carries the same "dados criam conteúdo" intent
/// (MASTER_HANDOFF seção 37-40) without that risk.
///
/// Deliberately does NOT have an equipment slot field yet — wiring items to
/// actual equip slots (Weapon/Offhand/Helmet/...) is Etapa 8. For now
/// <see cref="Tags"/> is enough to describe what an item *is*
/// (ex: "weapon", "sword", "melee", "physical", "currency"), matching the
/// tag list in MASTER_HANDOFF seção 18-24.
/// </summary>
public class ItemBaseDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Whether multiple copies of this base collapse into one Inventory
    /// slot with a count (ex: currency), instead of one slot each (ex:
    /// equipment). See <see cref="MaxStackSize"/> and
    /// <c>Game.Inventory.Inventory</c> (Etapa 7).
    /// </summary>
    public bool Stackable { get; set; }

    /// <summary>Ignored when <see cref="Stackable"/> is false.</summary>
    public int MaxStackSize { get; set; } = 1;

    /// <summary>
    /// Which equipment slot category this item goes in (Etapa 8).
    /// <see cref="EquipmentSlotCategory.None"/> (the default) means "not
    /// equipable" — currency/consumable/material bases leave this unset.
    /// </summary>
    public EquipmentSlotCategory EquipSlotCategory { get; set; } = EquipmentSlotCategory.None;

    /// <summary>
    /// Only meaningful for currency-like bases (Etapa 10): the id of the
    /// <c>ICraftingEffect</c> (see <c>Game.Crafting.CraftingEffectRegistry</c>)
    /// this item applies when used on a target. Null/empty means "not a
    /// crafting currency" — a data-driven binding instead of ever checking
    /// <c>if (item.Id == "dust_fragment")</c> in code, per MASTER_HANDOFF
    /// seção 37-40 ("nunca fazer if(itemName==...)").
    /// </summary>
    public string CraftingEffectId { get; set; }

    public bool HasTag(string tag) => Tags != null && Tags.Contains(tag);
    public bool IsEquipable => EquipSlotCategory != EquipmentSlotCategory.None;
    public bool IsCraftingCurrency => !string.IsNullOrEmpty(CraftingEffectId);
}
