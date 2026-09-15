using Game.Items;

namespace Game.Crafting;

/// <summary>Normal → Magic, rolling its natural 1 affix. Bound to "dust_fragment" in items.json (data, not code).</summary>
public class MakeMagicEffect : ICraftingEffect
{
    public bool CanApply(ItemInstance target, ItemBaseDefinition targetDefinition)
    {
        return targetDefinition != null && targetDefinition.IsEquipable && target.Rarity == ItemRarity.Normal;
    }

    public void Apply(ItemInstance target, ItemBaseDefinition targetDefinition)
    {
        target.Rarity = ItemRarity.Magic;
        AffixRoller.RollInitialAffixes(target, targetDefinition);
    }
}
