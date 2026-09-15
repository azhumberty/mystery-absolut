using Game.Items;

namespace Game.Crafting;

/// <summary>Adds one more affix if the item is under its rarity's cap. Bound to "crystal_amplification" in items.json.</summary>
public class AddModifierEffect : ICraftingEffect
{
    public bool CanApply(ItemInstance target, ItemBaseDefinition targetDefinition)
    {
        if (targetDefinition == null || !targetDefinition.IsEquipable)
        {
            return false;
        }

        bool isMagicOrRare = target.Rarity == ItemRarity.Magic || target.Rarity == ItemRarity.Rare;
        return isMagicOrRare && target.Affixes.Count < AffixRoller.MaxAffixesFor(target.Rarity);
    }

    public void Apply(ItemInstance target, ItemBaseDefinition targetDefinition)
    {
        AffixRoller.AddOneRandomAffix(target, targetDefinition);
    }
}
