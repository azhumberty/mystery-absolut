using Game.Items;

namespace Game.Crafting;

/// <summary>
/// One crafting operation a currency item can perform on a target
/// ItemInstance (MASTER_HANDOFF seção 37-40 — "CraftingCurrencyDefinition
/// → CraftingEffect", names given directly by that section:
/// MakeMagicEffect/RerollMagicModifiersEffect/AddModifierEffect/
/// UpgradeToRareEffect). Implementations never know which specific
/// currency item triggers them — that binding lives in
/// <see cref="ItemBaseDefinition.CraftingEffectId"/> (data), resolved via
/// <see cref="CraftingEffectRegistry"/>, so nothing here ever does
/// <c>if (itemName == "...")</c>.
/// </summary>
public interface ICraftingEffect
{
    bool CanApply(ItemInstance target, ItemBaseDefinition targetDefinition);
    void Apply(ItemInstance target, ItemBaseDefinition targetDefinition);
}
