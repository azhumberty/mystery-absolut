using System.Linq;
using Godot;
using Game.Items;
using Game.Inventory;

namespace Game.UI;

/// <summary>
/// "UI básica" for Stash (Etapa 13) — a second toggled panel (key
/// <c>stash</c>, T by default, independent from <c>inventory</c>'s I so
/// both can be open at the same time) showing the player's current
/// Inventory (click an item to store it) and the 4 Stash tabs (click a
/// stored item to retrieve it). Same code-built-Panel/Button approach as
/// <see cref="InventoryUI"/>, for the same reason (no `.tscn` Control
/// anchors/margins to hand-verify).
///
/// Deliberately reachable from anywhere, at any time — there's no home
/// base/NPC/location gating yet (that's future work; see
/// <c>Game.Inventory.Stash</c> doc for why the Stash itself lives where
/// it does for now).
/// </summary>
public partial class StashUI : CanvasLayer
{
    private VBoxContainer _inventoryList;
    private VBoxContainer _generalList;
    private VBoxContainer _equipmentList;
    private VBoxContainer _currencyList;
    private VBoxContainer _uniqueList;

    private Game.Inventory.Inventory _inventory;
    private Stash _stash;

    public override void _Ready()
    {
        BuildUi();
        Visible = false;
        Callable.From(() => ConnectToPlayer()).CallDeferred();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!@event.IsActionPressed("stash"))
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
            Position = new Vector2(380, 20),
            Size = new Vector2(340, 560),
        };
        AddChild(panel);

        var vbox = new VBoxContainer
        {
            Position = new Vector2(12, 12),
        };
        panel.AddChild(vbox);

        vbox.AddChild(new Label { Text = "Baú (T para fechar)" });
        vbox.AddChild(new HSeparator());
        vbox.AddChild(new Label { Text = "Inventário (clique para guardar)" });

        _inventoryList = new VBoxContainer();
        vbox.AddChild(_inventoryList);

        vbox.AddChild(new HSeparator());
        vbox.AddChild(new Label { Text = "Geral" });
        _generalList = new VBoxContainer();
        vbox.AddChild(_generalList);

        vbox.AddChild(new Label { Text = "Equipamento" });
        _equipmentList = new VBoxContainer();
        vbox.AddChild(_equipmentList);

        vbox.AddChild(new Label { Text = "Currency" });
        _currencyList = new VBoxContainer();
        vbox.AddChild(_currencyList);

        vbox.AddChild(new Label { Text = "Unique" });
        _uniqueList = new VBoxContainer();
        vbox.AddChild(_uniqueList);
    }

    private void ConnectToPlayer()
    {
        Node player = GetTree().GetFirstNodeInGroup("player");
        _inventory = player?.GetNodeOrNull<Game.Inventory.Inventory>("Inventory");
        _stash = player?.GetNodeOrNull<Stash>("Stash");

        if (_inventory == null || _stash == null)
        {
            GD.PushWarning("StashUI: could not find the player's Inventory/Stash node.");
            return;
        }

        _inventory.InventoryChanged += Refresh;
        _stash.StashChanged += Refresh;
        Refresh();
    }

    private void Refresh()
    {
        RefreshInventoryList();
        RefreshCategoryList(_generalList, StashCategory.General);
        RefreshCategoryList(_equipmentList, StashCategory.Equipment);
        RefreshCategoryList(_currencyList, StashCategory.Currency);
        RefreshCategoryList(_uniqueList, StashCategory.Unique);
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
            var button = new Button { Text = $"{DescribeItem(instance)}  [guardar]" };
            button.Pressed += () => _stash.Store(instance, _inventory);
            _inventoryList.AddChild(button);
        }
    }

    private void RefreshCategoryList(VBoxContainer list, StashCategory category)
    {
        foreach (Node child in list.GetChildren())
        {
            child.QueueFree();
        }

        if (_stash == null)
        {
            return;
        }

        var slots = _stash.GetSlots(category);
        if (slots.Count == 0)
        {
            list.AddChild(new Label { Text = "(vazio)" });
            return;
        }

        foreach (ItemInstance instance in slots)
        {
            var button = new Button { Text = $"{DescribeItem(instance)}  [retirar]" };
            button.Pressed += () => _stash.Retrieve(instance, _inventory);
            list.AddChild(button);
        }
    }

    private static string DescribeItem(ItemInstance instance)
    {
        ItemBaseDefinition definition = instance.GetBase();
        string name = definition?.Name ?? instance.BaseId;
        string rarity = instance.Rarity == ItemRarity.Normal ? "" : $" [{instance.Rarity}]";
        string count = instance.StackCount > 1 ? $" x{instance.StackCount}" : "";
        return $"{name}{rarity}{count}";
    }
}
