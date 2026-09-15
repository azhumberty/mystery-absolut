using System.Collections.Generic;
using System.Linq;
using Godot;
using Game.Items;

namespace Game.Inventory;

/// <summary>
/// Etapa 13 — separate, categorized storage (General/Equipment/Currency/
/// Unique per MASTER_HANDOFF seção 25-27: "Stash cedo no dev:
/// General/Equipment/Currency/Unique, depois busca/ordenação/tabs").
///
/// Same sibling-component pattern as Inventory/Equipment (child node
/// under Player.tscn) — **placeholder location, documented on purpose**:
/// a real Stash belongs to the player's account/save, not to the
/// character's on-person Node2D subtree, but there's no home base, no
/// persistence (Etapa 22) and no multi-character concept yet, so there's
/// nowhere else sensible to hang this state right now. The public API
/// here doesn't assume anything about *where* it lives, so relocating it
/// later (e.g. to a save-scoped autoload once Etapa 22 exists) is a move,
/// not a rewrite.
///
/// No search/sort/tabs UI logic here — that's explicitly "depois" per
/// MASTER_HANDOFF; <c>StashUI</c> just renders each category's
/// <see cref="GetSlots"/> list as-is, in insertion order. Category is
/// resolved automatically from item data (<see cref="ResolveCategory"/>)
/// — never a per-item-id hardcoded check.
/// </summary>
public partial class Stash : Node
{
    [Signal] public delegate void StashChangedEventHandler();

    [Export] public int CapacityPerCategory = 30;

    private readonly Dictionary<StashCategory, List<ItemInstance>> _slots = new()
    {
        { StashCategory.General, new List<ItemInstance>() },
        { StashCategory.Equipment, new List<ItemInstance>() },
        { StashCategory.Currency, new List<ItemInstance>() },
        { StashCategory.Unique, new List<ItemInstance>() },
    };

    public IReadOnlyList<ItemInstance> GetSlots(StashCategory category) => _slots[category];

    /// <summary>
    /// Moves <paramref name="instance"/> out of <paramref name="inventory"/>
    /// and into whichever stash tab its item data resolves to. Same
    /// check-room-before-removing safety as <c>Equipment.Equip</c> — if
    /// that tab is full (and, for stackable items, no existing stack has
    /// room either), nothing changes and the item stays in the inventory.
    /// </summary>
    public bool Store(ItemInstance instance, Inventory inventory)
    {
        if (instance == null || inventory == null)
        {
            return false;
        }

        ItemBaseDefinition definition = instance.GetBase();
        if (definition == null || !ContainsReference(inventory, instance))
        {
            return false;
        }

        if (!TryAdd(instance, definition))
        {
            return false;
        }

        inventory.RemoveItem(instance.InstanceId, instance.StackCount);
        return true;
    }

    /// <summary>Moves <paramref name="instance"/> back from the stash into <paramref name="inventory"/>, if there's room there.</summary>
    public bool Retrieve(ItemInstance instance, Inventory inventory)
    {
        if (instance == null || inventory == null)
        {
            return false;
        }

        StashCategory category = ResolveCategory(instance, instance.GetBase());
        List<ItemInstance> slots = _slots[category];
        if (!slots.Contains(instance))
        {
            return false;
        }

        if (!inventory.AddItem(instance))
        {
            return false;
        }

        slots.Remove(instance);
        EmitSignal(SignalName.StashChanged);
        return true;
    }

    private bool TryAdd(ItemInstance instance, ItemBaseDefinition definition)
    {
        StashCategory category = ResolveCategory(instance, definition);
        List<ItemInstance> slots = _slots[category];

        if (definition.Stackable)
        {
            ItemInstance existingStack = slots.FirstOrDefault(slot =>
                slot.BaseId == instance.BaseId && slot.StackCount + instance.StackCount <= definition.MaxStackSize);
            if (existingStack != null)
            {
                existingStack.StackCount += instance.StackCount;
                EmitSignal(SignalName.StashChanged);
                return true;
            }
        }

        if (slots.Count >= CapacityPerCategory)
        {
            return false;
        }

        slots.Add(instance);
        EmitSignal(SignalName.StashChanged);
        return true;
    }

    /// <summary>
    /// Unique first (an equipable Unique still goes to the Unique tab, not
    /// Equipment), then Equipment, then Currency, everything else General.
    /// Driven entirely by data already on the item
    /// (Rarity/IsEquipable/IsCraftingCurrency) — no per-item-id checks.
    /// </summary>
    private static StashCategory ResolveCategory(ItemInstance instance, ItemBaseDefinition definition)
    {
        if (instance.Rarity == ItemRarity.Unique)
        {
            return StashCategory.Unique;
        }

        if (definition != null && definition.IsEquipable)
        {
            return StashCategory.Equipment;
        }

        if (definition != null && definition.IsCraftingCurrency)
        {
            return StashCategory.Currency;
        }

        return StashCategory.General;
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

    // Etapa 22 helpers
    public void ForceAdd(ItemInstance instance)
    {
        if (instance != null)
        {
            ItemBaseDefinition definition = instance.GetBase();
            if (definition != null) TryAdd(instance, definition);
        }
    }

    public List<ItemInstance> GetAllItems()
    {
        List<ItemInstance> all = new();
        foreach (var kvp in _slots) all.AddRange(kvp.Value);
        return all;
    }

    public void Clear()
    {
        foreach (var kvp in _slots) kvp.Value.Clear();
    }
}
