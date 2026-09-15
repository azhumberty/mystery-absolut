namespace Game.Items;

/// <summary>
/// What kind of slot an <see cref="ItemBaseDefinition"/> is *for*
/// (<see cref="ItemBaseDefinition.EquipSlotCategory"/>) — separate from
/// <see cref="EquipmentSlot"/>, which is the character's actual concrete
/// slots. They're split into two enums because of Ring: there are two
/// concrete ring slots (<c>Ring1</c>/<c>Ring2</c>) but only one item
/// category ("this is a ring, it goes in whichever ring slot is free") —
/// a ring item shouldn't need to declare which of the two slots it
/// prefers. Every other category maps 1:1 to a single concrete slot; see
/// <c>Equipment.ResolveSlot</c> for the resolution rule.
/// </summary>
public enum EquipmentSlotCategory
{
    /// <summary>Not an equipable item (currency, consumable, material, ...).</summary>
    None,
    Weapon,
    Offhand,
    Helmet,
    Chest,
    Gloves,
    Boots,
    Amulet,
    Ring,
}
