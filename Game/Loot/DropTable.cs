using System;

namespace Game.Loot;

/// <summary>
/// Minimal, generic drop table: doesn't reference any specific item type
/// yet — ItemBase/ItemInstance/Rarity don't exist until Etapa 6/9, so
/// this only decides whether "generic loot" drops at all, and how many
/// pickups. Plain C# class, not a Godot Resource: a custom Resource
/// subclass would need hand-authored sub_resource blocks in .tscn files,
/// which is a serialization format I can't fully verify without the
/// editor open — the same reasoning that kept InputMap bindings in code
/// instead of raw project.godot entries. Owning nodes (EnemyController)
/// expose the numbers below as plain [Export] fields instead, and build
/// a DropTable from them at runtime.
/// </summary>
public class DropTable
{
    public float DropChance;
    public int MinDrops;
    public int MaxDrops;

    public DropTable(float dropChance, int minDrops, int maxDrops)
    {
        DropChance = dropChance;
        MinDrops = Math.Max(0, minDrops);
        MaxDrops = Math.Max(MinDrops, maxDrops);
    }

    /// <summary>How many generic loot drops should spawn (0 if the chance roll fails).</summary>
    public int Roll(Random random)
    {
        if (random.NextDouble() > DropChance)
        {
            return 0;
        }

        return random.Next(MinDrops, MaxDrops + 1);
    }
}
