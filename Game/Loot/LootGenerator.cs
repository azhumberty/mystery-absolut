using System.Collections.Generic;
using System.Linq;
using Godot;
using Game.Items;

namespace Game.Loot;

/// <summary>
/// Turns a DropTable roll into actual GroundItem nodes in the world, each
/// carrying a real <see cref="ItemInstance"/> since Etapa 6. Deliberately
/// the only place that knows how to instantiate GroundItem AND the only
/// place that knows how to roll an item for a drop, so future changes
/// (weighted per-enemy drop pools, rarity-based presentation effects)
/// touch this one file instead of every actor that can drop loot.
///
/// **Placeholder scope, documented on purpose:** base selection is a
/// uniform random pick across the *entire* <see cref="ItemDatabase"/> —
/// there's no per-enemy drop pool yet (that's weighted pools, Etapa 9-ish
/// territory per MASTER_HANDOFF seção 41-42, "DropTable com pesos,
/// pools..."). Rarity is a simple weighted roll (Normal/Magic/Rare only —
/// Unique is deliberately excluded: a real Unique is a specific,
/// hand-designed item with fixed stats, not "any generic base + Unique
/// tag", and that system doesn't exist yet). <see cref="ItemInstance.ItemLevel"/>
/// is hardcoded to <see cref="PlaceholderItemLevel"/> because there's no
/// enemy/area level system yet — it exists on the data model now (Etapa 6
/// asked for it) purely so Etapa 9 (Affixes) has a field to read later.
/// </summary>
public partial class LootGenerator : Node
{
    [Export] public PackedScene GroundItemScene;

    private const int PlaceholderItemLevel = 1;

    private static readonly System.Random Random = new();

    public void Generate(DropTable table, Vector2 worldPosition, Node parent)
    {
        int count = table.Roll(Random);
        for (int i = 0; i < count; i++)
        {
            SpawnOne(worldPosition, parent);
        }
    }

    private void SpawnOne(Vector2 worldPosition, Node parent)
    {
        if (GroundItemScene == null)
        {
            GD.PushWarning("LootGenerator: GroundItemScene not set, cannot spawn loot.");
            return;
        }

        ItemInstance instance = RollItemInstance();
        if (instance == null)
        {
            GD.PushWarning("LootGenerator: ItemDatabase has no items, nothing to drop.");
            return;
        }

        var groundItem = GroundItemScene.Instantiate<GroundItem>();
        groundItem.Payload = instance;
        parent.AddChild(groundItem);
        groundItem.GlobalPosition = worldPosition + RandomOffset();
    }

    private static ItemInstance RollItemInstance()
    {
        IReadOnlyCollection<ItemBaseDefinition> all = ItemDatabase.Instance.GetAll();
        if (all.Count == 0)
        {
            return null;
        }

        ItemBaseDefinition chosen = all.ElementAt(Random.Next(all.Count));
        // Stackable bases (currency/consumable) never roll Magic/Rare —
        // rarity/affixes only make sense on equipable items.
        ItemRarity rarity = chosen.Stackable ? ItemRarity.Normal : RollRarity();
        int stackCount = chosen.Stackable ? Random.Next(1, 4) : 1;

        var instance = new ItemInstance(chosen.Id, rarity, PlaceholderItemLevel, stackCount);
        AffixRoller.RollInitialAffixes(instance, chosen);
        return instance;
    }

    /// <summary>Normal 70% / Magic 25% / Rare 5%. See class doc for why Unique is excluded.</summary>
    private static ItemRarity RollRarity()
    {
        double roll = Random.NextDouble();
        if (roll < 0.70)
        {
            return ItemRarity.Normal;
        }

        return roll < 0.95 ? ItemRarity.Magic : ItemRarity.Rare;
    }

    private static Vector2 RandomOffset()
    {
        float angle = (float)(Random.NextDouble() * Mathf.Tau);
        float distance = (float)Random.NextDouble() * 20f;
        return Vector2.FromAngle(angle) * distance;
    }
}
