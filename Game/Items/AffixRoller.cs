using System.Collections.Generic;
using System.Linq;

namespace Game.Items;

/// <summary>
/// All the "roll me an affix" logic in one place — used both when loot
/// first drops (<c>LootGenerator</c>) and when a crafting effect adds/
/// rerolls modifiers (<c>Game.Crafting</c>, Etapa 10). Keeping this
/// separate from both callers avoids duplicating the weighted-pick/cap
/// logic in two places that would drift apart.
///
/// **Deliberately simple, documented placeholder rules** (MASTER_HANDOFF
/// seção 18-24 just asks for "Prefix/Suffix, tiers, pools, weighted
/// rolls" — this is the minimal version of that, not the final balance):
/// Normal has 0 affixes, Magic caps at 2 (1 prefix + 1 suffix), Rare caps
/// at 4 (2 prefix + 2 suffix). A fresh drop rolls 1 affix for Magic, 2 for
/// Rare — but which type(s) depends on which pool still has room, so a
/// Rare's natural 2 rolls are NOT guaranteed to be "1 prefix + 1 suffix",
/// just "up to 2 total, evenly capped per type". Unique isn't rolled this
/// way at all (see LootGenerator's doc for why).
/// </summary>
public static class AffixRoller
{
    private static readonly System.Random Random = new();

    public static int MaxAffixesFor(ItemRarity rarity)
    {
        if (rarity == ItemRarity.Magic)
        {
            return 2;
        }

        if (rarity == ItemRarity.Rare)
        {
            return 4;
        }

        return 0;
    }

    private static int NaturalRollCountFor(ItemRarity rarity)
    {
        if (rarity == ItemRarity.Magic)
        {
            return 1;
        }

        if (rarity == ItemRarity.Rare)
        {
            return 2;
        }

        return 0;
    }

    /// <summary>Clears and rolls the affixes an item gets the moment it's created (used by LootGenerator).</summary>
    public static void RollInitialAffixes(ItemInstance instance, ItemBaseDefinition definition)
    {
        instance.Affixes.Clear();
        int count = NaturalRollCountFor(instance.Rarity);
        for (int i = 0; i < count; i++)
        {
            AddOneRandomAffix(instance, definition);
        }
    }

    /// <summary>
    /// Adds exactly one more affix (prefix or suffix, whichever still has
    /// room under the rarity's per-type cap and has an eligible
    /// candidate), never duplicating an affix the item already has.
    /// Returns false — instance unchanged — if nothing could be added
    /// (cap reached, or no eligible candidates left).
    /// </summary>
    public static bool AddOneRandomAffix(ItemInstance instance, ItemBaseDefinition definition)
    {
        int cap = MaxAffixesFor(instance.Rarity);
        if (instance.Affixes.Count >= cap)
        {
            return false;
        }

        int maxPerType = cap / 2;
        int prefixCount = instance.Affixes.Count(a => a.GetDefinition()?.Type == AffixType.Prefix);
        int suffixCount = instance.Affixes.Count - prefixCount;

        var candidateTypes = new List<AffixType>();
        if (prefixCount < maxPerType)
        {
            candidateTypes.Add(AffixType.Prefix);
        }

        if (suffixCount < maxPerType)
        {
            candidateTypes.Add(AffixType.Suffix);
        }

        if (candidateTypes.Count == 0)
        {
            return false;
        }

        AffixType chosenType = candidateTypes[Random.Next(candidateTypes.Count)];
        List<AffixDefinition> pool = AffixDatabase.Instance
            .GetEligiblePool(definition, chosenType, instance.ItemLevel)
            .Where(candidate => instance.Affixes.All(existing => existing.DefinitionId != candidate.Id))
            .ToList();

        if (pool.Count == 0)
        {
            return false;
        }

        AffixDefinition picked = WeightedPick(pool);
        double value = picked.MinValue + Random.NextDouble() * (picked.MaxValue - picked.MinValue);
        instance.Affixes.Add(new AffixInstance(picked.Id, value));
        return true;
    }

    /// <summary>Clears every affix and rolls the same count fresh (used by RerollMagicModifiersEffect).</summary>
    public static void RerollAffixes(ItemInstance instance, ItemBaseDefinition definition)
    {
        int count = instance.Affixes.Count;
        instance.Affixes.Clear();
        for (int i = 0; i < count; i++)
        {
            AddOneRandomAffix(instance, definition);
        }
    }

    private static AffixDefinition WeightedPick(List<AffixDefinition> pool)
    {
        int totalWeight = pool.Sum(a => a.Weight);
        if (totalWeight <= 0)
        {
            return pool[Random.Next(pool.Count)];
        }

        int roll = Random.Next(totalWeight);
        int cumulative = 0;
        foreach (AffixDefinition definition in pool)
        {
            cumulative += definition.Weight;
            if (roll < cumulative)
            {
                return definition;
            }
        }

        return pool[^1];
    }
}
