using Game.Items;

namespace Game.Crafting;

/// <summary>
/// Entry point for "use this currency on this item". Deliberately does
/// NOT touch any Inventory — consuming the currency (removing 1 from
/// wherever it came from) is the caller's responsibility, once this
/// returns true. Keeps this class a pure item-mutation operation, easy to
/// reuse from UI, a future crafting bench, etc.
/// </summary>
public static class CraftingService
{
    public static bool TryApply(ItemBaseDefinition currencyDefinition, ItemInstance target, ItemBaseDefinition targetDefinition)
    {
        if (currencyDefinition == null || !currencyDefinition.IsCraftingCurrency || target == null || targetDefinition == null)
        {
            return false;
        }

        ICraftingEffect effect = CraftingEffectRegistry.Get(currencyDefinition.CraftingEffectId);
        if (effect == null || !effect.CanApply(target, targetDefinition))
        {
            return false;
        }

        effect.Apply(target, targetDefinition);
        return true;
    }
}
