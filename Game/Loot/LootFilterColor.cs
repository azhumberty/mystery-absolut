using Godot;

namespace Game.Loot;

/// <summary>
/// A JSON-friendly color — plain R/G/B/A doubles (0-1), not a hex string.
/// Deliberately avoids any `Color(string)` HTML-code parsing API: the
/// 4-float `Color` constructor is already proven safe elsewhere in this
/// codebase (every `.tscn` uses it), while a hex-string constructor/parser
/// has never been exercised here and isn't worth the extra unverified
/// surface just to make <c>Data/Loot/loot_filter.json</c> slightly
/// prettier to read.
/// </summary>
public class LootFilterColor
{
    public double R { get; set; }
    public double G { get; set; }
    public double B { get; set; }
    public double A { get; set; } = 1.0;

    public Color ToColor()
    {
        return new Color((float)R, (float)G, (float)B, (float)A);
    }
}
