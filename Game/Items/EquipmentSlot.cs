namespace Game.Items;

/// <summary>
/// The concrete slots a character has (MASTER_HANDOFF seção 25-27):
/// Weapon, Offhand, Helmet, Chest, Gloves, Boots, Amulet, Ring1, Ring2.
/// Used by <see cref="Game.Inventory.Equipment"/>'s slot dictionary. Not
/// the same thing as <see cref="EquipmentSlotCategory"/> — see that enum's
/// doc for why they're separate.
/// </summary>
public enum EquipmentSlot
{
    Weapon,
    Offhand,
    Helmet,
    Chest,
    Gloves,
    Boots,
    Amulet,
    Ring1,
    Ring2,
}
