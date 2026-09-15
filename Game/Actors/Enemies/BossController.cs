using Godot;
using Game.Combat;
using Game.Loot;
using Game.Quests;

namespace Game.Actors.Enemies;

public partial class BossController : CharacterBody2D
{
    public enum State
    {
        Idle,
        Chase,
        Attack,
        Telegraph,
        AoeAttack,
        Recover,
        Dead
    }

    [Export] public float DetectionRange = 400f;
    [Export] public float LeashRange = 500f;
    [Export] public float AttackRange = 45f;
    [Export] public float MoveSpeed = 80f; // Slower initially
    
    [Export] public float NormalAttackDuration = 0.3f;
    [Export] public float AoeTelegraphDuration = 1.2f;
    [Export] public float AoeAttackDuration = 0.5f;
    [Export] public float RecoverDuration = 0.8f;
    [Export] public float NormalDamage = 20f;
    [Export] public float AoeDamage = 40f;

    [Export] public float LootDropChance = 1.0f;
    [Export] public int MinLootDrops = 6;
    [Export] public int MaxLootDrops = 10;

    public State CurrentState { get; private set; } = State.Idle;
    public bool IsPhaseTwo { get; private set; } = false;

    private Node2D _player;
    private Health _health;
    private Hitbox _hitbox;
    private Hitbox _aoeHitbox;
    private Polygon2D _visual;
    private Polygon2D _telegraphVisual;
    private LootGenerator _lootGenerator;
    private double _stateTimer;

    public override void _Ready()
    {
        AddToGroup("enemy");
        _player = GetTree().GetFirstNodeInGroup("player") as Node2D;
        _health = GetNode<Health>("Health");
        _hitbox = GetNode<Hitbox>("AttackHitbox");
        _aoeHitbox = GetNode<Hitbox>("AoeHitbox");
        _visual = GetNode<Polygon2D>("Placeholder");
        _telegraphVisual = GetNode<Polygon2D>("TelegraphVisual");
        _lootGenerator = GetNodeOrNull<LootGenerator>("LootGenerator");

        _hitbox.Owner2D = this;
        _hitbox.Damage = NormalDamage;
        
        _aoeHitbox.Owner2D = this;
        _aoeHitbox.Damage = AoeDamage;
        _telegraphVisual.Visible = false;

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
        CheckPhaseTransition();

        switch (CurrentState)
        {
            case State.Idle:
                Velocity = Vector2.Zero;
                if (distance <= DetectionRange) CurrentState = State.Chase;
                break;

            case State.Chase:
                if (distance > LeashRange)
                {
                    CurrentState = State.Idle;
                    Velocity = Vector2.Zero;
                }
                else if (distance <= AttackRange)
                {
                    StartNormalAttack();
                }
                else if (IsPhaseTwo && distance <= DetectionRange * 0.5f && GD.Randf() < 0.02f)
                {
                    // 2% chance per frame when close in phase 2 to trigger AoE
                    StartAoeTelegraph();
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

            case State.Telegraph:
                Velocity = Vector2.Zero;
                _stateTimer -= delta;
                
                // Pulsing red effect
                _telegraphVisual.Modulate = new Color(1f, 0f, 0f, 0.3f + 0.5f * Mathf.Sin((float)_stateTimer * 10f));
                
                if (_stateTimer <= 0.0)
                {
                    ExecuteAoeAttack();
                }
                break;

            case State.AoeAttack:
                Velocity = Vector2.Zero;
                _stateTimer -= delta;
                if (_stateTimer <= 0.0)
                {
                    _aoeHitbox.Deactivate();
                    _telegraphVisual.Visible = false;
                    CurrentState = State.Recover;
                    _stateTimer = RecoverDuration * 1.5f;
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

    private void CheckPhaseTransition()
    {
        if (!IsPhaseTwo && _health.CurrentHealth <= _health.MaxHealth * 0.5f)
        {
            IsPhaseTwo = true;
            MoveSpeed = 130f; // Enrages! Faster speed.
            _visual.Color = new Color(0.8f, 0.1f, 0.1f); // Dark red
        }
    }

    private void StartNormalAttack()
    {
        CurrentState = State.Attack;
        Vector2 direction = (_player.GlobalPosition - GlobalPosition).Normalized();
        _hitbox.Position = direction * (AttackRange - 10f);
        _hitbox.Activate();
        _stateTimer = NormalAttackDuration;
    }

    private void StartAoeTelegraph()
    {
        CurrentState = State.Telegraph;
        _stateTimer = AoeTelegraphDuration;
        _telegraphVisual.Visible = true;
        _telegraphVisual.Modulate = new Color(1f, 0f, 0f, 0.3f);
    }

    private void ExecuteAoeAttack()
    {
        CurrentState = State.AoeAttack;
        _telegraphVisual.Modulate = new Color(1f, 0f, 0f, 0.8f); // Solid red
        _aoeHitbox.Activate();
        _stateTimer = AoeAttackDuration;
    }

    private void OnDamaged(float amount, float currentHealth)
    {
        if (CurrentState == State.Dead) return;
        float ratio = _health.MaxHealth > 0f ? currentHealth / _health.MaxHealth : 0f;
        Color baseColor = IsPhaseTwo ? new Color(0.8f, 0.1f, 0.1f) : new Color(1f, 0.4f, 0.2f);
        _visual.Modulate = baseColor * new Color(1f, 1f, 1f, Mathf.Clamp(0.3f + 0.7f * ratio, 0.3f, 1f));
    }

    private void OnDied()
    {
        CurrentState = State.Dead;
        Velocity = Vector2.Zero;
        SetDeferred(CollisionObject2D.PropertyName.CollisionLayer, 0);
        SetDeferred(CollisionObject2D.PropertyName.CollisionMask, 0);
        _hitbox.Deactivate();
        _aoeHitbox.Deactivate();
        _telegraphVisual.Visible = false;
        
        _visual.Modulate = new Color(0.2f, 0.2f, 0.2f, 0.5f);
        SetPhysicsProcess(false);

        _lootGenerator?.Generate(
            new DropTable(LootDropChance, MinLootDrops, MaxLootDrops),
            GlobalPosition,
            GetParent());
            
        // Drop guaranteed epic/legendary maybe? 
        // We'll rely on the high drop count.

        QuestManager.ReportObjectiveProgress("kill_boss", 1);
        
        var player = GetTree().GetFirstNodeInGroup("player");
        if (player != null)
        {
            player.GetNodeOrNull<Combat.Stats>("Stats")?.AddXP(150f);
        }
    }
}
