using Game.Items;

namespace Game.Crafting;

/// <summary>Rerolls a Magic item's existing affixes fresh. Bound to "shard_reforging" in items.json.</summary>
public class RerollMagicModifiersEffect : ICraftingEffect
{
    public bool CanApply(ItemInstance target, ItemBaseDefinition targetDefinition)
    {
        return targetDefinition != null && target.Rarity == ItemRarity.Magic;
    }

    public void Apply(ItemInstance target, ItemBaseDefinition targetDefinition)
    {
        AffixRoller.RerollAffixes(target, targetDefinition);
    }
}
