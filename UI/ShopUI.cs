using Godot;
using Game.Inventory;
using Game.Items;

namespace Game.UI;

public partial class ShopUI : CanvasLayer
{
    private Panel _panel;
    private ItemList _shopList;
    private Label _goldLabel;
    
    private Gold _playerGold;
    private Inventory.Inventory _playerInventory;

    public override void _Ready()
    {
        _panel = new Panel
        {
            CustomMinimumSize = new Vector2(300, 400),
            Position = new Vector2(50, 100),
            Visible = false
        };
        AddChild(_panel);

        var vbox = new VBoxContainer { CustomMinimumSize = new Vector2(280, 380), Position = new Vector2(10, 10) };
        _panel.AddChild(vbox);

        vbox.AddChild(new Label { Text = "Merchant Shop" });
        _goldLabel = new Label { Text = "Gold: 0" };
        vbox.AddChild(_goldLabel);

        _shopList = new ItemList { CustomMinimumSize = new Vector2(280, 300) };
        _shopList.ItemActivated += OnShopItemActivated;
        vbox.AddChild(_shopList);

        Callable.From(() => ConnectToPlayer()).CallDeferred();
    }

    private void ConnectToPlayer()
    {
        var player = GetTree().GetFirstNodeInGroup("player");
        if (player != null)
        {
            _playerGold = player.GetNodeOrNull<Gold>("Gold");
            _playerInventory = player.GetNodeOrNull<Inventory.Inventory>("Inventory");
            
            if (_playerGold != null)
            {
                _playerGold.GoldChanged += (int g) => UpdateGold();
                UpdateGold();
            }
        }
    }

    private void UpdateGold()
    {
        if (_playerGold != null)
        {
            _goldLabel.Text = $"Gold: {_playerGold.CurrentGold}";
        }
    }

    public void OpenShop()
    {
        _panel.Visible = true;
        PopulateShop();
    }

    public void CloseShop()
    {
        _panel.Visible = false;
    }

    private void PopulateShop()
    {
        _shopList.Clear();
        // Just add a generic potion for 50G
        _shopList.AddItem("Health Potion (50G)");
        _shopList.SetItemMetadata(0, 50);
    }

    private void OnShopItemActivated(long index)
    {
        int cost = (int)_shopList.GetItemMetadata((int)index);
        
        if (_playerGold != null && _playerGold.TrySpendGold(cost))
        {
            if (_playerInventory != null)
            {
                // Fake a generic item drop, in reality we'd pull from DB.
                // Assuming we just want to prove the shop works:
                var potion = new ItemInstance("consumable_health_potion", ItemRarity.Normal, 1, 1);
                _playerInventory.AddItem(potion);
            }
        }
    }
}
