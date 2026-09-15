using Godot;
using Game.Items;

namespace Game.Loot;

/// <summary>
/// Physical loot sitting in the world. Since Etapa 6, carries a real
/// <see cref="Payload"/> (<see cref="ItemInstance"/>) set by whoever
/// instantiates it (today: only <c>LootGenerator</c>) right after
/// spawning — walking over it tries to add that instance to the player's
/// <see cref="Game.Inventory.Inventory"/> (sibling node under Player,
/// found by name, same convention as other actor sub-components) and only
/// disappears if the inventory actually had room; otherwise it just stays
/// on the ground, matching "não implementar preenchimento parcial" already
/// decided in Inventory.AddItem. Emits Collected so future systems (loot
/// presentation, quest triggers) can hook in without touching this file.
/// Uses the default physics layer/mask (1) deliberately, so it reacts to
/// any physics body — the IsInGroup("player") check below is what
/// actually restricts collection to the player, rather than a dedicated
/// collision layer, since nothing else needs to interact with loot yet.
/// </summary>
public partial class GroundItem : Area2D
{
    [Signal] public delegate void CollectedEventHandler();

    /// <summary>
    /// Set by the spawner (LootGenerator) immediately after
    /// Instantiate()/AddChild(), before the first physics frame can
    /// trigger a pickup. Never [Export]ed — an ItemInstance is a plain C#
    /// object with a constructor-only InstanceId, not something the
    /// editor/Inspector could meaningfully set anyway.
    /// </summary>
    public ItemInstance Payload { get; set; }

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        ApplyPresentation();
    }

    /// <summary>
    /// Etapa 11+12 — builds the label/outline/glow/beam/particles child
    /// nodes for this item, and decides whether it's shown at all, from
    /// <see cref="LootFilterDatabase.Evaluate"/> (Etapa 12's rules, falling
    /// back to Etapa 11's plain rarity defaults when nothing matches).
    /// Built entirely in code rather than authored into GroundItem.tscn —
    /// same risk-avoidance reasoning already used for InventoryUI: precise
    /// Control anchors/margins and multiple new node types are much safer
    /// to get right as plain property assignments than as hand-written
    /// `.tscn` blocks nobody can open in the editor to double-check here.
    /// No-ops when <see cref="Payload"/> is null (the Etapa-5-style
    /// generic fallback pickup has no rarity/name to present).
    /// </summary>
    private void ApplyPresentation()
    {
        if (Payload == null)
        {
            return;
        }

        ItemBaseDefinition definition = Payload.GetBase();
        LootFilterResult result = LootFilterDatabase.Instance.Evaluate(Payload, definition);

        // "Hide" (Etapa 12) means invisible but still walkable/collectible —
        // Area2D collision detection is independent of CanvasItem.Visible in
        // Godot, so this never blocks OnBodyEntered from firing.
        Visible = result.Show;

        // FontColor doubles as "this item's accent color" for the ground
        // square itself, not just the label text — there's no separate
        // "ItemColor" action because nothing so far needs the square and
        // the label to ever show different colors.
        var visual = GetNodeOrNull<Polygon2D>("Visual");
        if (visual != null)
        {
            visual.Color = result.FontColor;
        }

        BuildLabel(definition, result);

        if (result.ShowOutline)
        {
            BuildOutline(result);
        }

        if (result.ShowGlow)
        {
            BuildGlow(result);
        }

        if (result.ShowBeam)
        {
            BuildBeam(result);
        }

        if (result.ShowParticles)
        {
            BuildParticles(result);
        }
    }

    private void BuildLabel(ItemBaseDefinition definition, LootFilterResult result)
    {
        string name = definition?.Name ?? "(item desconhecido)";
        string suffix = Payload.StackCount > 1 ? $" x{Payload.StackCount}" : string.Empty;

        var label = new Label();
        label.Text = name + suffix;
        label.Position = new Vector2(-60, -26);
        label.Size = new Vector2(120, 16);
        label.HorizontalAlignment = HorizontalAlignment.Center;
        label.ZIndex = 10;
        label.AddThemeColorOverride("font_color", result.FontColor);
        label.AddThemeFontSizeOverride("font_size", result.FontSize);
        AddChild(label);
    }

    /// <summary>Thin ring behind the item's own Polygon2D, slightly bigger, same color family.</summary>
    private void BuildOutline(LootFilterResult result)
    {
        var outline = new Polygon2D();
        outline.Color = result.OutlineColor;
        outline.Polygon = new Vector2[]
        {
            new Vector2(-8, -8), new Vector2(8, -8), new Vector2(8, 8), new Vector2(-8, 8),
        };
        outline.ZIndex = -1;
        AddChild(outline);
    }

    /// <summary>Bigger, semi-transparent square behind everything, pulsing in/out forever via Tween.</summary>
    private void BuildGlow(LootFilterResult result)
    {
        bool big = Payload.Rarity == ItemRarity.Unique;
        float half = big ? 20f : 14f;

        var glow = new Polygon2D();
        glow.Color = result.GlowColor;
        glow.Polygon = new Vector2[]
        {
            new Vector2(-half, -half), new Vector2(half, -half), new Vector2(half, half), new Vector2(-half, half),
        };
        glow.ZIndex = -2;
        glow.Modulate = new Color(1f, 1f, 1f, 0.15f);
        AddChild(glow);

        Tween tween = CreateTween();
        tween.BindNode(glow);
        tween.SetLoops();
        tween.TweenProperty(glow, "modulate:a", big ? 0.6 : 0.45, 0.6)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.InOut);
        tween.TweenProperty(glow, "modulate:a", 0.15, 0.6)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.InOut);
    }

    /// <summary>Thin, semi-transparent column extending upward — bigger for Unique than Rare.</summary>
    private void BuildBeam(LootFilterResult result)
    {
        bool tall = Payload.Rarity == ItemRarity.Unique;
        float height = tall ? 100f : 60f;
        float width = tall ? 5f : 3f;

        var beam = new Polygon2D();
        beam.Color = new Color(result.BeamColor.R, result.BeamColor.G, result.BeamColor.B, 0.35f);
        beam.Polygon = new Vector2[]
        {
            new Vector2(-width, -4), new Vector2(width, -4), new Vector2(width, -4 - height), new Vector2(-width, -4 - height),
        };
        beam.ZIndex = -1;
        AddChild(beam);
    }

    /// <summary>Etapa 11's "partículas básicas".</summary>
    private void BuildParticles(LootFilterResult result)
    {
        var particles = new CpuParticles2D();
        particles.Emitting = true;
        particles.Amount = 14;
        particles.Lifetime = 1.2;
        particles.Explosiveness = 0.0f;
        particles.Direction = new Vector2(0, -1);
        particles.Spread = 25.0f;
        particles.Gravity = new Vector2(0, -18);
        particles.Color = result.GlowColor;
        particles.ScaleAmountMin = 1.2f;
        particles.ScaleAmountMax = 2.0f;
        particles.ZIndex = 5;
        AddChild(particles);
    }

    private void OnBodyEntered(Node2D body)
    {
        if (!body.IsInGroup("player"))
        {
            return;
        }

        if (Payload == null)
        {
            GD.PushWarning("GroundItem: no Payload set, collecting anyway (Etapa 5-style generic pickup).");
        }
        else
        {
            var inventory = body.GetNodeOrNull<Game.Inventory.Inventory>("Inventory");
            if (inventory == null)
            {
                GD.PushWarning("GroundItem: player has no Inventory node, cannot collect.");
                return;
            }

            if (!inventory.AddItem(Payload))
            {
                // Inventory full: leave the item on the ground.
                return;
            }
        }

        EmitSignal(SignalName.Collected);
        GD.Print($"Loot coletado: {Payload?.GetBase()?.Name ?? "(sem item)"}" + (Payload != null && Payload.StackCount > 1 ? $" x{Payload.StackCount}" : ""));
        QueueFree();
    }
}
