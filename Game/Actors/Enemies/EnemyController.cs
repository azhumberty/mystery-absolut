using Godot;
using Game.Combat;
using Game.Loot;

namespace Game.Actors.Enemies;

/// <summary>
/// First enemy: deliberately simple AI (MASTER_HANDOFF section 47 —
/// "começar simples"). A plain enum + switch is enough for one enemy
/// type with one attack; Patrol/Flee/Summon/PhaseTransition and a real
/// state-machine framework are explicitly future work, not needed yet.
///
/// Reuses the exact same Game.Combat components the player uses
/// (Health, Combatant, Hurtbox, Hitbox) unchanged — this is the payoff of
/// keeping those decoupled from PlayerController in Etapa 2: an enemy is
/// just another actor that owns them.
/// </summary>
public partial class EnemyController : CharacterBody2D
{
    public enum State
    {
        Idle,
        Chase,
        Attack,
        Recover,
        Dead
    }

    [Export] public float DetectionRange = 180f;
    [Export] public float LeashRange = 260f;
    [Export] public float AttackRange = 34f;
    [Export] public float MoveSpeed = 90f;
    [Export] public float AttackDuration = 0.2f;
    [Export] public float RecoverDuration = 0.5f;
    [Export] public float Damage = 10f;

    [Export] public float LootDropChance = 1.0f;
    [Export] public int MinLootDrops = 2;
    [Export] public int MaxLootDrops = 4;

    public State CurrentState { get; private set; } = State.Idle;

    private Node2D _player;
    private Health _health;
    private Hitbox _hitbox;
    private Polygon2D _visual;
    private LootGenerator _lootGenerator;
    private double _stateTimer;

    public override void _Ready()
    {
        _player = GetTree().GetFirstNodeInGroup("player") as Node2D;
        _health = GetNode<Health>("Health");
        _hitbox = GetNode<Hitbox>("AttackHitbox");
        _visual = GetNode<Polygon2D>("Placeholder");
        _lootGenerator = GetNodeOrNull<LootGenerator>("LootGenerator");

        _hitbox.Owner2D = this;
        _hitbox.Damage = Damage;

        _health.Damaged += OnDamaged;
        _health.Died += OnDied;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (CurrentState == State.Dead || _player == null)
        {
            Velocity = Vector2.Zero;
            MoveAndSlide();
            return;
        }

        float distance = GlobalPosition.DistanceTo(_player.GlobalPosition);

        switch (CurrentState)
        {
            case State.Idle:
                Velocity = Vector2.Zero;
                if (distance <= DetectionRange)
                {
                    CurrentState = State.Chase;
                }
                break;

            case State.Chase:
                if (distance > LeashRange)
                {
                    CurrentState = State.Idle;
                    Velocity = Vector2.Zero;
                }
                else if (distance <= AttackRange)
                {
                    StartAttack();
                }
                else
                {
                    Velocity = (_player.GlobalPosition - GlobalPosition).Normalized() * MoveSpeed;
                }
                break;

            case State.Attack:
                Velocity = Vector2.Zero;
                _stateTimer -= delta;
                if (_stateTimer <= 0.0)
                {
                    _hitbox.Deactivate();
                    CurrentState = State.Recover;
                    _stateTimer = RecoverDuration;
                }
                break;

            case State.Recover:
                Velocity = Vector2.Zero;
                _stateTimer -= delta;
                if (_stateTimer <= 0.0)
                {
                    CurrentState = distance <= DetectionRange ? State.Chase : State.Idle;
                }
                break;
        }

        MoveAndSlide();
    }

    private void StartAttack()
    {
        CurrentState = State.Attack;
        Vector2 direction = (_player.GlobalPosition - GlobalPosition).Normalized();
        _hitbox.Position = direction * (AttackRange - 4f);
        _hitbox.Activate();
        _stateTimer = AttackDuration;
    }

    /// <summary>
    /// Minimal, still-placeholder visual feedback (fade proportional to
    /// remaining HP) so combat state is at least legible without a real
    /// "Hit Feedback" system (future item) or real character art (see
    /// MASTER_CONTEXT.md "Direção visual" — explicitly deferred).
    /// </summary>
    private void OnDamaged(float amount, float currentHealth)
    {
        float ratio = _health.MaxHealth > 0f ? currentHealth / _health.MaxHealth : 0f;
        _visual.Modulate = new Color(1f, 1f, 1f, Mathf.Clamp(0.3f + 0.7f * ratio, 0.3f, 1f));
    }

    private void OnDied()
    {
        CurrentState = State.Dead;
        Velocity = Vector2.Zero;
        SetDeferred(CollisionObject2D.PropertyName.CollisionLayer, 0);
        SetDeferred(CollisionObject2D.PropertyName.CollisionMask, 0);
        _hitbox.Deactivate();
        _visual.Modulate = new Color(0.4f, 0.4f, 0.4f, 0.5f);
        SetPhysicsProcess(false);

        _lootGenerator?.Generate(
            new DropTable(LootDropChance, MinLootDrops, MaxLootDrops),
            GlobalPosition,
            GetParent());
    }
}
