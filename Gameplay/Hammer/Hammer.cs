using Godot;
using WhacAGuy.Gameplay.Controllers;

public partial class Hammer : Area2D
{
    private Timer _attackTimer;

    private AttackRoundController _round;
    private EventBus _eventBus;

    private float _timeInside = 0f;
    private bool _isPlayerInArea = false;

    private float _attackDelay = 0.5f;

    public void Init(EventBus eventBus, AttackRoundController round)
    {
        _eventBus = eventBus;
        _round = round;
    }

    public void Configure(float attackDelay)
    {
        _attackDelay = attackDelay;

        if (_attackTimer != null)
            _attackTimer.WaitTime = _attackDelay;
    }

    public override void _Ready()
    {
        _attackTimer = GetNode<Timer>("BeforeAttackTimer");
        _attackTimer.WaitTime = _attackDelay;
        GD.Print($"HAMMER DELAY = {_attackDelay}");
        _attackTimer.Timeout += OnAttackTimerTimeout;

        AreaEntered += OnHammerAttackZoneEntered;
        AreaExited += OnHammerAttackZoneExited;

        SetProcess(false);

        _attackTimer.Start();
    }

    public override void _Process(double delta)
    {
        if (!_isPlayerInArea)
            return;

        _timeInside += (float)delta;
    }

    private void OnAttackTimerTimeout()
    {
        if (_isPlayerInArea)
        {
            _round?.RegisterHit();
            _eventBus.EmitPlayerGotHit();
        }

        _round?.NotifyHammerFinished(
            _timeInside,
            _attackDelay);

        QueueFree();
    }

    private void OnHammerAttackZoneEntered(Area2D area)
    {
        _isPlayerInArea = true;
        SetProcess(true);
    }

    private void OnHammerAttackZoneExited(Area2D area)
    {
        _isPlayerInArea = false;
        SetProcess(false);
    }
}