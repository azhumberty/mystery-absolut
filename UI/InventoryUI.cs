using System.Linq;
using Godot;
using Game.Items;
using Game.Crafting;

namespace Game.UI;

/// <summary>
/// "UI básica" for Inventory (Etapa 7), extended for Equipment (Etapa 8)
/// and Crafting (Etapa 10): a single toggled panel listing inventory
/// contents, equipped items per slot, and — for crafting currencies — a
/// button that applies their effect. Still no grid/icons/drag-and-drop;
/// everything interactive is a plain <see cref="Button"/> with text,
/// built in code for the same reason as always (anchors/margins in raw
/// .tscn aren't verifiable without the editor).
///
/// **Crafting interaction is a deliberate placeholder, not a real UI**:
/// pressing a currency button applies its effect to the *first eligible
/// item still in the inventory* (see <see cref="FindCraftableTarget"/>) —
/// there's no item-picking/targeting UI, because nothing in the roadmap
/// asked for one yet (Etapa 10's bullet is just "primeiros
/// CraftingEffects... nomes temporários somente em dados"). This exists
/// so the system is exercised by pressing a button in-game rather than
/// only by code that never runs.
/// </summary>
public partial class InventoryUI : CanvasLayer
{
    private VBoxContainer _inventoryList;
    private VBoxContainer _equippedList;
    private Game.Inventory.Inventory _inventory;
    private Game.Inventory.Equipment _equipment;

    private static readonly EquipmentSlot[] AllSlots =
    {
        EquipmentSlot.Weapon, EquipmentSlot.Offhand, EquipmentSlot.Helmet,
        EquipmentSlot.Chest, EquipmentSlot.Gloves, EquipmentSlot.Boots,
        EquipmentSlot.Amulet, EquipmentSlot.Ring1, EquipmentSlot.Ring2,
    };

    public override void _Ready()
    {
        BuildUi();
        Visible = false;
        Callable.From(() => ConnectToPlayer()).CallDeferred();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!@event.IsActionPressed("inventory"))
        {
            return;
        }

        Visible = !Visible;
        if (Visible)
        {
            Refresh();
        }
    }

    private void BuildUi()
    {
        var panel = new Panel
        {
            Position = new Vector2(20, 20),
            Size = new Vector2(340, 520),
        };
        AddChild(panel);

        var vbox = new VBoxContainer
        {
            Position = new Vector2(12, 12),
        };
        panel.AddChild(vbox);

        vbox.AddChild(new Label { Text = "Inventário (I para fechar)" });
        vbox.AddChild(new HSeparator());

        _inventoryList = new VBoxContainer();
        vbox.AddChild(_inventoryList);

        vbox.AddChild(new HSeparator());
        vbox.AddChild(new Label { Text = "Equipado" });

        _equippedList = new VBoxContainer();
        vbox.AddChild(_equippedList);
    }

    private void ConnectToPlayer()
    {
        Node player = GetTree().GetFirstNodeInGroup("player");
        _inventory = player?.GetNodeOrNull<Game.Inventory.Inventory>("Inventory");
        _equipment = player?.GetNodeOrNull<Game.Inventory.Equipment>("Equipment");

        if (_inventory == null || _equipment == null)
        {
            GD.PushWarning("InventoryUI: could not find the player's Inventory/Equipment node.");
            return;
        }

        _inventory.InventoryChanged += Refresh;
        _equipment.EquipmentChanged += Refresh;
        Refresh();
    }

    private void Refresh()
    {
        RefreshInventoryList();
        RefreshEquippedList();
    }

    private void RefreshInventoryList()
    {
        foreach (Node child in _inventoryList.GetChildren())
        {
            child.QueueFree();
        }

        if (_inventory == null || _inventory.Slots.Count == 0)
        {
            _inventoryList.AddChild(new Label { Text = "(vazio)" });
            return;
        }

        foreach (ItemInstance instance in _inventory.Slots)
        {
            ItemBaseDefinition definition = instance.GetBase();
            string text = DescribeItem(instance, definition);

            if (definition != null && definition.IsEquipable)
            {
                var button = new Button { Text = text };
                button.Pressed += () => _equipment.Equip(instance, _inventory);
                _inventoryList.AddChild(button);
            }
            else if (definition != null && definition.IsCraftingCurrency)
            {
                var button = new Button { Text = $"{text}  [usar]" };
                button.Pressed += () => TryCraft(instance, definition);
                _inventoryList.AddChild(button);
            }
            else
            {
                _inventoryList.AddChild(new Label { Text = text });
            }
        }
    }

    private void RefreshEquippedList()
    {
        foreach (Node child in _equippedList.GetChildren())
        {
            child.QueueFree();
        }

        if (_equipment == null)
        {
            return;
        }

        foreach (EquipmentSlot slot in AllSlots)
        {
            ItemInstance equipped = _equipment.GetEquipped(slot);
            if (equipped == null)
            {
                _equippedList.AddChild(new Label { Text = $"{slot}: (vazio)" });
                continue;
            }

            var button = new Button { Text = $"{slot}: {DescribeItem(equipped, equipped.GetBase())}" };
            button.Pressed += () => _equipment.Unequip(slot, _inventory);
            _equippedList.AddChild(button);
        }
    }

    private void TryCraft(ItemInstance currency, ItemBaseDefinition currencyDefinition)
    {
        ItemInstance target = FindCraftableTarget(currencyDefinition);
        if (target == null)
        {
            GD.Print($"Nenhum item elegível para {currencyDefinition.Name}.");
            return;
        }

        if (!CraftingService.TryApply(currencyDefinition, target, target.GetBase()))
        {
            GD.Print($"{currencyDefinition.Name} não pôde ser aplicado.");
            return;
        }

        _inventory.RemoveItem(currency.InstanceId, 1);
        GD.Print($"{currencyDefinition.Name} aplicado em {target.GetBase()?.Name}.");
        Refresh();
    }

    private ItemInstance FindCraftableTarget(ItemBaseDefinition currencyDefinition)
    {
        ICraftingEffect effect = CraftingEffectRegistry.Get(currencyDefinition.CraftingEffectId);
        if (effect == null)
        {
            return null;
        }

        return _inventory.Slots.FirstOrDefault(candidate =>
        {
            ItemBaseDefinition candidateDefinition = candidate.GetBase();
            return candidateDefinition != null
                && candidateDefinition.IsEquipable
                && effect.CanApply(candidate, candidateDefinition);
        });
    }

    private static string DescribeItem(ItemInstance instance, ItemBaseDefinition definition)
    {
        string name = definition?.Name ?? instance.BaseId;
        string rarity = instance.Rarity == ItemRarity.Normal ? "" : $" [{instance.Rarity}]";
        string count = instance.StackCount > 1 ? $" x{instance.StackCount}" : "";
        string affixes = instance.Affixes.Count == 0
            ? ""
            : "\n" + string.Join("\n", instance.Affixes.Select(a => $"  {a.ToDisplayText()}"));
        return $"{name}{rarity}{count}{affixes}";
    }
}
