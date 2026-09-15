using System.Collections.Generic;
using Godot;
using Game.Items;

namespace Game.Inventory;

/// <summary>
/// Holds what the Player currently has equipped, one <see cref="ItemInstance"/>
/// per <see cref="EquipmentSlot"/> (Etapa 8 — "todos os slots"). Sibling
/// component under Player.tscn, same composition pattern as
/// <see cref="Inventory"/>/Health/Combatant.
///
/// **Scope note:** this only manages *which item sits in which slot* —
/// equipping something does NOT yet change any player stat (damage,
/// defense, etc.). That needs a "compute effective stats from equipped
/// items + their affixes" system that nothing in the roadmap has asked
/// for by name yet; MASTER_HANDOFF seção 62 lists Etapa 8 as just "todos
/// os slots". Wiring a weapon's damage into `PlayerBasicAttack`, for
/// example, is a deliberate non-goal here — documented instead of
/// silently done, per the "não implementar sistemas futuros
/// antecipadamente" rule.
/// </summary>
public partial class Equipment : Node
{
    [Signal] public delegate void EquipmentChangedEventHandler();

    private readonly Dictionary<EquipmentSlot, ItemInstance> _equipped = new();

    public IReadOnlyDictionary<EquipmentSlot, ItemInstance> Equipped => _equipped;

    public ItemInstance GetEquipped(EquipmentSlot slot)
    {
        return _equipped.TryGetValue(slot, out ItemInstance instance) ? instance : null;
    }

    /// <summary>
    /// Moves <paramref name="instance"/> from <paramref name="inventory"/>
    /// into its resolved slot. If that slot is occupied, the previous
    /// occupant goes back to the inventory first — and if there's no room
    /// for it there, the whole operation aborts and nothing changes (no
    /// partial swap, no item lost). <paramref name="instance"/> must
    /// currently be present in <paramref name="inventory"/> (this doesn't
    /// pull items from anywhere else).
    /// </summary>
    public bool Equip(ItemInstance instance, Inventory inventory)
    {
        if (instance == null || inventory == null)
        {
            return false;
        }

        ItemBaseDefinition definition = instance.GetBase();
        if (definition == null || !definition.IsEquipable)
        {
            return false;
        }

        if (!ContainsReference(inventory, instance))
        {
            return false;
        }

        EquipmentSlot slot = ResolveSlot(definition.EquipSlotCategory);

        ItemInstance previous = GetEquipped(slot);
        if (previous != null && !inventory.AddItem(previous))
        {
            // No room to give the previous item back: abort, change nothing.
            return false;
        }

        inventory.RemoveItem(instance.InstanceId, instance.StackCount);
        _equipped[slot] = instance;
        EmitSignal(SignalName.EquipmentChanged);
        return true;
    }

    /// <summary>Moves whatever is in <paramref name="slot"/> back to the inventory, if there's room.</summary>
    public bool Unequip(EquipmentSlot slot, Inventory inventory)
    {
        if (inventory == null || !_equipped.TryGetValue(slot, out ItemInstance instance))
        {
            return false;
        }

        if (!inventory.AddItem(instance))
        {
            return false;
        }

        _equipped.Remove(slot);
        EmitSignal(SignalName.EquipmentChanged);
        return true;
    }

    /// <summary>
    /// Every category maps to exactly one concrete slot except Ring: with
    /// two ring slots and one "ring" category, this picks the first empty
    /// one, or Ring1 if both are already full (so equipping a second ring
    /// deterministically replaces Ring1, not "whichever" — matters for
    /// the swap-back logic in <see cref="Equip"/>).
    /// </summary>
    private EquipmentSlot ResolveSlot(EquipmentSlotCategory category)
    {
        switch (category)
        {
            case EquipmentSlotCategory.Weapon:
                return EquipmentSlot.Weapon;
            case EquipmentSlotCategory.Offhand:
                return EquipmentSlot.Offhand;
            case EquipmentSlotCategory.Helmet:
                return EquipmentSlot.Helmet;
            case EquipmentSlotCategory.Chest:
                return EquipmentSlot.Chest;
            case EquipmentSlotCategory.Gloves:
                return EquipmentSlot.Gloves;
            case EquipmentSlotCategory.Boots:
                return EquipmentSlot.Boots;
            case EquipmentSlotCategory.Amulet:
                return EquipmentSlot.Amulet;
            case EquipmentSlotCategory.Ring:
                return _equipped.ContainsKey(EquipmentSlot.Ring1) ? EquipmentSlot.Ring2 : EquipmentSlot.Ring1;
            default:
                return EquipmentSlot.Weapon;
        }
    }

    private static bool ContainsReference(Inventory inventory, ItemInstance instance)
    {
        foreach (ItemInstance slot in inventory.Slots)
        {
            if (ReferenceEquals(slot, instance))
            {
                return true;
            }
        }

        return false;
    }
}
