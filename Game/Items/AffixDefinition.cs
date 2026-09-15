using System.Collections.Generic;

namespace Game.Items;

/// <summary>
/// Shared, read-only template for one possible affix (ex: "+X Dano
/// Físico") — loaded once from <c>Data/Items/affixes.json</c> by
/// <see cref="AffixDatabase"/>, the same JSON-not-Resource approach as
/// <see cref="ItemBaseDefinition"/>/<see cref="ItemDatabase"/> and for the
/// same reason. A rolled, item-specific copy of this is an
/// <see cref="AffixInstance"/>.
/// </summary>
public class AffixDefinition
{
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Display text with a <c>{0}</c> placeholder for the rolled value
    /// (ex: <c>"+{0} Dano Físico"</c>), formatted by
    /// <see cref="AffixInstance.ToDisplayText"/>.
    /// </summary>
    public string NameTemplate { get; set; } = string.Empty;

    public AffixType Type { get; set; }

    /// <summary>
    /// 1 = melhor/mais forte (convenção do gênero: tier mais baixo é mais
    /// poderoso). Não limita nada sozinho — combinado com
    /// <see cref="RequiredItemLevel"/> é o que controla quando o afixo
    /// pode aparecer.
    /// </summary>
    public int Tier { get; set; } = 1;

    /// <summary>
    /// Minimum <see cref="ItemInstance.ItemLevel"/> required for this
    /// affix to be eligible. Every entry in the current catalog uses 1
    /// because nothing in the game assigns a higher ItemLevel yet (see
    /// <c>LootGenerator.PlaceholderItemLevel</c>) — the field exists now
    /// so a future leveling system unlocks higher tiers without any
    /// change here.
    /// </summary>
    public int RequiredItemLevel { get; set; } = 1;

    /// <summary>
    /// An item is eligible for this affix only if its
    /// <see cref="ItemBaseDefinition.Tags"/> contains at least one of
    /// these (ex: an affix tagged ["weapon"] never rolls on armor).
    /// </summary>
    public List<string> RequiredTags { get; set; } = new();

    public double MinValue { get; set; }
    public double MaxValue { get; set; }

    /// <summary>Relative weight for weighted random selection within its pool — higher rolls more often.</summary>
    public int Weight { get; set; } = 100;
}
