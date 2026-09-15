using Godot;
using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using Game.Items;
using Game.Inventory;
using Game.Narrative;
using Game.Quests;

namespace Game.Core;

public class SaveData
{
    public Dictionary<string, int> WorldStateData { get; set; } = new();
    public List<string> ActiveQuests { get; set; } = new();
    // Simplified Inventory save
    public List<ItemInstance> PlayerInventory { get; set; } = new();
    public List<ItemInstance> PlayerEquipment { get; set; } = new();
    public List<ItemInstance> Stash { get; set; } = new();
}

public static class SaveManager
{
    private const string SavePath = "user://savegame.json";

// Same logic applied with correct names
    public static void SaveGame(Node tree)
    {
        var data = new SaveData();

        data.WorldStateData = WorldState.GetAll();
        data.ActiveQuests = new List<string>(QuestManager.ActiveQuests);

        var player = tree.GetTree().GetFirstNodeInGroup("player");
        if (player != null)
        {
            var inv = player.GetNodeOrNull<Inventory.Inventory>("Inventory");
            if (inv != null)
            {
                data.PlayerInventory = new List<ItemInstance>(inv.Slots);
            }
            
            var equip = player.GetNodeOrNull<Inventory.Equipment>("Equipment");
            if (equip != null)
            {
                foreach (var eq in equip.Equipped.Values)
                {
                    if (eq != null) data.PlayerEquipment.Add(eq);
                }
            }
        }

        var stashNode = tree.GetTree().GetFirstNodeInGroup("player")?.GetNodeOrNull<Inventory.Stash>("Stash");
        if (stashNode != null)
        {
            data.Stash = stashNode.GetAllItems();
        }

        try
        {
            string globalPath = ProjectSettings.GlobalizePath(SavePath);
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(globalPath, json);
            GD.Print("Game Saved to " + globalPath);
        }
        catch (Exception e)
        {
            GD.PrintErr("Failed to save game: " + e.Message);
        }
    }

    public static void LoadGame(Node tree)
    {
        string globalPath = ProjectSettings.GlobalizePath(SavePath);
        if (!File.Exists(globalPath))
        {
            GD.Print("No save game found.");
            return;
        }

        try
        {
            string json = File.ReadAllText(globalPath);
            var data = JsonSerializer.Deserialize<SaveData>(json);
            if (data == null) return;

            WorldState.SetAll(data.WorldStateData);

            foreach (var q in data.ActiveQuests)
            {
                QuestManager.AcceptQuest(q);
            }

            var player = tree.GetTree().GetFirstNodeInGroup("player");
            if (player != null)
            {
                var inv = player.GetNodeOrNull<Inventory.Inventory>("Inventory");
                if (inv != null)
                {
                    // Basic clear (to simulate full reload)
                    foreach (var s in new List<ItemInstance>(inv.Slots))
                    {
                        inv.RemoveItem(s.InstanceId, s.StackCount);
                    }
                    foreach (var item in data.PlayerInventory)
                    {
                        inv.AddItem(item);
                    }
                }

                // Equipment is skipped for now in this quick iteration
                
                var stashNode = player.GetNodeOrNull<Inventory.Stash>("Stash");
                if (stashNode != null)
                {
                    stashNode.Clear();
                    foreach (var item in data.Stash)
                    {
                        stashNode.ForceAdd(item);
                    }
                }
            }

            GD.Print("Game Loaded.");
        }
        catch (Exception e)
        {
            GD.PrintErr("Failed to load game: " + e.Message);
        }
    }
}
