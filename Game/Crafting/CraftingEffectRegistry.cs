using System.Collections.Generic;

namespace Game.Crafting;

/// <summary>
/// Maps a <see cref="Game.Items.ItemBaseDefinition.CraftingEffectId"/>
/// string (data, from items.json) to the actual
/// <see cref="ICraftingEffect"/> instance. This one file is the only
/// place a currency id-string is ever tied to behavior — everything else
/// (LootGenerator, CraftingService, UI) only ever sees the interface.
/// </summary>
public static class CraftingEffectRegistry
{
    private static readonly Dictionary<string, ICraftingEffect> ById = new()
    {
        ["make_magic"] = new MakeMagicEffect(),
        ["reroll_magic"] = new RerollMagicModifiersEffect(),
        ["add_modifier"] = new AddModifierEffect(),
        ["upgrade_to_rare"] = new UpgradeToRareEffect(),
    };

    public static ICraftingEffect Get(string id)
    {
        return !string.IsNullOrEmpty(id) && ById.TryGetValue(id, out ICraftingEffect effect) ? effect : null;
    }
}
