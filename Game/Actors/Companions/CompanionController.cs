using Godot;
using Game.Combat;

namespace Game.Actors.Companions;

/// <summary>
/// Controlador do primeiro companion (Etapa 14).
/// Segue o jogador e ataca inimigos próximos automaticamente.
/// Reaproveita a infraestrutura de combate (Health, Combatant, Hurtbox, Hitbox)
/// e possui um estado temporário `Downed` quando o HP chega a zero (revive depois de um tempo).
/// </summary>
public partial class CompanionController : CharacterBody2D
{
    public enum State
    {
        Idle,
        Follow,
        Attack,
        Recover,
        Downed,
        Retreat,
        Regroup
    }

    [Export] public float FollowRange = 60f;
    [Export] public float StopRange = 40f;
    [Export] public float DetectionRange = 180f;
    [Export] public float AttackRange = 36f;
    [Export] public float MoveSpeed = 200f;
    [Export] public float AttackDuration = 0.2f;
    [Export] public float RecoverDuration = 0.5f;
    [Export] public float DownedDuration = 10f; // Tempo para reviver automaticamente

    public State CurrentState { get; private set; } = State.Idle;

    private Node2D _player;
    private Node2D _targetEnemy;
    private Health _health;
    private Hitbox _hitbox;
    private Polygon2D _visual;
    private double _stateTimer;
    private Color _originalColor = new Color(0.2f, 0.8f, 0.2f); // Verde

    public override void _Ready()
    {
        AddToGroup("companion");

        _player = GetTree().GetFirstNodeInGroup("player") as Node2D;
        _health = GetNode<Health>("Health");
        _hitbox = GetNode<Hitbox>("AttackHitbox");
        _visual = GetNode<Polygon2D>("Visual");

        _visual.Modulate = _originalColor;

        _health.Damaged += OnDamaged;
        _health.Died += OnDied;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_player == null)
        {
            return;
        }

        if (CurrentState == State.Downed)
        {
            _stateTimer -= delta;
            if (_stateTimer <= 0.0)
            {
                Revive();
            }
            Velocity = Vector2.Zero;
            MoveAndSlide();
            return;
        }

        if (Input.IsActionJustPressed("companion_regroup"))
        {
            CurrentState = State.Regroup;
            _targetEnemy = null;
        }
        else if (CurrentState != State.Regroup && _health.CurrentHealth / _health.MaxHealth <= 0.2f && CurrentState != State.Retreat)
        {
            CurrentState = State.Retreat;
            _targetEnemy = null;
        }

        UpdateTargetEnemy();

        float distanceToPlayer = GlobalPosition.DistanceTo(_player.GlobalPosition);
        float distanceToEnemy = _targetEnemy != null ? GlobalPosition.DistanceTo(_targetEnemy.GlobalPosition) : float.MaxValue;

        switch (CurrentState)
        {
            case State.Idle:
                Velocity = Vector2.Zero;
                if (_targetEnemy != null && distanceToEnemy <= DetectionRange)
                {
                    CurrentState = State.Attack;
                }
                else if (distanceToPlayer > FollowRange)
                {
                    CurrentState = State.Follow;
                }
                break;

            case State.Follow:
                if (_targetEnemy != null && distanceToEnemy <= DetectionRange)
                {
                    CurrentState = State.Attack;
                }
                else if (distanceToPlayer <= StopRange)
                {
                    CurrentState = State.Idle;
                    Velocity = Vector2.Zero;
                }
                else
                {
                    Velocity = (_player.GlobalPosition - GlobalPosition).Normalized() * MoveSpeed;
                }
                break;

            case State.Attack:
                if (_targetEnemy == null || !IsInstanceValid(_targetEnemy))
                {
                    CurrentState = State.Idle;
                    Velocity = Vector2.Zero;
                    break;
                }

                if (distanceToEnemy <= AttackRange)
                {
                    StartAttack();
                }
                else if (distanceToPlayer > FollowRange * 2) 
                {
                    CurrentState = State.Follow;
                }
                else
                {
                    Velocity = (_targetEnemy.GlobalPosition - GlobalPosition).Normalized() * MoveSpeed;
                }
                break;

            case State.Recover:
                Velocity = Vector2.Zero;
                _stateTimer -= delta;
                if (_stateTimer <= 0.0)
                {
                    CurrentState = State.Idle;
                }
                break;

            case State.Retreat:
                if (_health.CurrentHealth / _health.MaxHealth > 0.2f)
                {
                    CurrentState = State.Idle;
                }
                else if (_targetEnemy != null)
                {
                    // Run away from enemy
                    Velocity = (GlobalPosition - _targetEnemy.GlobalPosition).Normalized() * MoveSpeed;
                    
                    // But don't stray too far from player
                    if (distanceToPlayer > FollowRange * 1.5f)
                    {
                        Velocity = (_player.GlobalPosition - GlobalPosition).Normalized() * MoveSpeed;
                    }
                }
                else if (distanceToPlayer > StopRange)
                {
                    Velocity = (_player.GlobalPosition - GlobalPosition).Normalized() * MoveSpeed;
                }
                else
                {
                    Velocity = Vector2.Zero;
                }
                break;

            case State.Regroup:
                if (distanceToPlayer <= StopRange)
                {
                    CurrentState = State.Idle;
                    Velocity = Vector2.Zero;
                }
                else
                {
                    Velocity = (_player.GlobalPosition - GlobalPosition).Normalized() * MoveSpeed * 1.2f; // Run slightly faster
                }
                break;
        }

        MoveAndSlide();
    }

    private void UpdateTargetEnemy()
    {
        if (_targetEnemy != null && IsInstanceValid(_targetEnemy))
        {
            var enemyHealth = _targetEnemy.GetNodeOrNull<Health>("Health");
            if (enemyHealth != null && enemyHealth.CurrentHealth <= 0)
            {
                _targetEnemy = null;
            }
            else
            {
                return;
            }
        }

        _targetEnemy = null;
        float closestDistance = DetectionRange;

        foreach (Node node in GetTree().GetNodesInGroup("enemy"))
        {
            if (node is Node2D enemyNode)
            {
                var enemyHealth = enemyNode.GetNodeOrNull<Health>("Health");
                if (enemyHealth != null && enemyHealth.CurrentHealth <= 0)
                {
                    continue;
                }

                float dist = GlobalPosition.DistanceTo(enemyNode.GlobalPosition);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    _targetEnemy = enemyNode;
                }
            }
        }
    }

    private void StartAttack()
    {
        CurrentState = State.Recover;
        Vector2 direction = (_targetEnemy.GlobalPosition - GlobalPosition).Normalized();
        _hitbox.Position = direction * (AttackRange - 4f);
        _hitbox.Activate();
        
        _stateTimer = AttackDuration + RecoverDuration;
        
        GetTree().CreateTimer(AttackDuration).Timeout += () => 
        {
            if (IsInstanceValid(this) && _hitbox != null)
            {
                _hitbox.Deactivate();
            }
        };
    }

    private void OnDamaged(float amount, float currentHealth)
    {
        float ratio = _health.MaxHealth > 0f ? currentHealth / _health.MaxHealth : 0f;
        _visual.Modulate = _originalColor * new Color(1f, 1f, 1f, Mathf.Clamp(0.3f + 0.7f * ratio, 0.3f, 1f));
    }

    private void OnDied()
    {
        CurrentState = State.Downed;
        Velocity = Vector2.Zero;
        SetDeferred(CollisionObject2D.PropertyName.CollisionLayer, 0);
        SetDeferred(CollisionObject2D.PropertyName.CollisionMask, 0);
        _hitbox.Deactivate();
        
        _visual.Modulate = new Color(0.4f, 0.4f, 0.4f, 0.5f);
        _stateTimer = DownedDuration;
    }

    private void Revive()
    {
        CurrentState = State.Idle;
        _health.Revive(0.5f);
        
        SetDeferred(CollisionObject2D.PropertyName.CollisionLayer, 1);
        SetDeferred(CollisionObject2D.PropertyName.CollisionMask, 1);
        
        _visual.Modulate = _originalColor;
    }
}
