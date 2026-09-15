using Game.Items;

namespace Game.Crafting;

/// <summary>Magic → Rare, keeping existing affixes and rolling one more. Bound to "orb_ascension" in items.json.</summary>
public class UpgradeToRareEffect : ICraftingEffect
{
    public bool CanApply(ItemInstance target, ItemBaseDefinition targetDefinition)
    {
        return targetDefinition != null && target.Rarity == ItemRarity.Magic;
    }

    public void Apply(ItemInstance target, ItemBaseDefinition targetDefinition)
    {
        target.Rarity = ItemRarity.Rare;
        AffixRoller.AddOneRandomAffix(target, targetDefinition);
    }
}
