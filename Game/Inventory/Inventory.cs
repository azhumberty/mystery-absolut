using System.Collections.Generic;
using System.Linq;
using Godot;
using Game.Items;

namespace Game.Inventory;

/// <summary>
/// Holds a fixed number of item slots for whoever owns it (today: only the
/// Player, as a sibling component under <c>Player.tscn</c> — same
/// composition pattern as Health/Combatant/Hurtbox). Etapa 7 scope only:
/// stack, add ("pickup"), remove, and a way for UI to know something
/// changed. No equip slots (Etapa 8), no sorting/search/tabs (that's
/// Stash, Etapa 13), no weight/currency-tab split yet.
///
/// Stacking key is <see cref="ItemBaseDefinition.Id"/> alone (ignores
/// Rarity/ItemLevel) — simple and correct for now because the only
/// <see cref="ItemBaseDefinition.Stackable"/> bases in
/// <c>Data/Items/items.json</c> (currency/material) don't vary by rarity in
/// any way that matters yet. If a future stackable base *does* need
/// rarity-aware stacking, that's a small change to <see cref="CanMergeInto"/>,
/// not a redesign.
/// </summary>
public partial class Inventory : Node
{
    [Signal] public delegate void InventoryChangedEventHandler();

    [Export] public int Capacity = 20;

    private readonly List<ItemInstance> _slots = new();

    public IReadOnlyList<ItemInstance> Slots => _slots;

    /// <summary>
    /// Tries to fit the whole instance into the inventory: first into an
    /// existing compatible stack with room, otherwise into a new slot.
    /// Deliberately all-or-nothing (no partial fills splitting one pickup
    /// across two stacks) — keeps the logic simple for this stage; nothing
    /// today drops stacks large enough for that to matter in practice.
    /// Returns false (instance untouched, caller keeps it — ex: GroundItem
    /// stays on the ground) when there's no room at all.
    /// </summary>
    public bool AddItem(ItemInstance instance)
    {
        if (instance == null)
        {
            return false;
        }

        ItemBaseDefinition definition = instance.GetBase();
        if (definition == null)
        {
            GD.PushWarning($"Inventory: item base '{instance.BaseId}' not found, refusing to add.");
            return false;
        }

        if (definition.Stackable)
        {
            ItemInstance existingStack = _slots.FirstOrDefault(slot => CanMergeInto(slot, instance, definition));
            if (existingStack != null)
            {
                existingStack.StackCount += instance.StackCount;
                EmitSignal(SignalName.InventoryChanged);
                return true;
            }
        }

        if (_slots.Count >= Capacity)
        {
            return false;
        }

        _slots.Add(instance);
        EmitSignal(SignalName.InventoryChanged);
        return true;
    }

    /// <summary>
    /// Removes up to <paramref name="amount"/> units of the given instance
    /// (by InstanceId). Returns how many were actually removed (0 if the
    /// instance isn't present) — callers that need "all or nothing" check
    /// the return value against what they asked for.
    /// </summary>
    public int RemoveItem(string instanceId, int amount)
    {
        if (amount <= 0)
        {
            return 0;
        }

        ItemInstance slot = _slots.FirstOrDefault(s => s.InstanceId == instanceId);
        if (slot == null)
        {
            return 0;
        }

        int removed = Mathf.Min(amount, slot.StackCount);
        slot.StackCount -= removed;
        if (slot.StackCount <= 0)
        {
            _slots.Remove(slot);
        }

        EmitSignal(SignalName.InventoryChanged);
        return removed;
    }

    private static bool CanMergeInto(ItemInstance existing, ItemInstance incoming, ItemBaseDefinition definition)
    {
        return existing.BaseId == incoming.BaseId
            && existing.StackCount + incoming.StackCount <= definition.MaxStackSize;
    }
}
