using System.Globalization;

namespace Game.Items;

/// <summary>
/// One rolled affix sitting on a specific <see cref="ItemInstance"/> —
/// points at its <see cref="AffixDefinition"/> by id (same
/// pointer-not-copy pattern as <see cref="ItemInstance.BaseId"/>) plus the
/// concrete value this particular roll landed on.
/// </summary>
public class AffixInstance
{
    public string DefinitionId { get; set; }
    public double RolledValue { get; set; }

    public AffixInstance() { }

    public AffixInstance(string definitionId, double rolledValue)
    {
        DefinitionId = definitionId;
        RolledValue = rolledValue;
    }

    public AffixDefinition GetDefinition() => AffixDatabase.Instance.Get(DefinitionId);

    /// <summary>Formats using the definition's NameTemplate, rounding the value to at most 1 decimal place.</summary>
    public string ToDisplayText()
    {
        AffixDefinition definition = GetDefinition();
        if (definition == null)
        {
            return DefinitionId;
        }

        string valueText = RolledValue % 1 == 0
            ? RolledValue.ToString("0", CultureInfo.InvariantCulture)
            : RolledValue.ToString("0.0", CultureInfo.InvariantCulture);

        return string.Format(CultureInfo.InvariantCulture, definition.NameTemplate, valueText);
    }
}
