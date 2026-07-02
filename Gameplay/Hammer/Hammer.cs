using Godot;
using WhacAGuy.Gameplay.Controllers;

public partial class Hammer : Area2D
{
    private Timer _attackTimer;

    private AttackRoundController _round;
    private EventBus _eventBus;
    private Player _player;

    private float _attackDelay = 0.5f;
    private bool _dodged;
    private bool _inParryWindow;
    private Sprite2D _hammerSprite;
    private Tween _parryTween;
    private bool _paused;

    private double _pausedTimeLeft;
    private float _ringProgress;
    public float RingProgress
    {
        get => _ringProgress;
        set { _ringProgress = value; QueueRedraw(); }
    }

    public void Init(EventBus eventBus, AttackRoundController round, Player player)
    {
        _eventBus = eventBus;
        _round = round;
        _player = player;
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
        _attackTimer.Timeout += OnAttackTimerTimeout;

        var collision = GetNode<CollisionShape2D>("EnemyCollision");
        if (collision.Shape is CircleShape2D circle)
            circle.Radius = 80f;

        _hammerSprite = GetNode<Sprite2D>("HammerZone");

        AreaEntered += OnHammerAttackZoneEntered;
        AreaExited += OnHammerAttackZoneExited;

        _eventBus.Connect(EventBus.SignalName.MoveButtonClicked,
            new Callable(this, nameof(OnMoveButtonClicked)));

        _eventBus.Connect(EventBus.SignalName.GamePaused,
            new Callable(this, nameof(OnGamePaused)));
        _eventBus.Connect(EventBus.SignalName.GameUnpaused,
            new Callable(this, nameof(OnGameUnpaused)));

        StartParryIndicator();

        _attackTimer.Start();
    }

    private void StartParryIndicator()
    {
        float window = Mathf.Min(0.3f, _attackDelay * 0.4f);
        float startTime = Mathf.Max(0.05f, _attackDelay - window);

        SceneTreeTimer timer = GetTree().CreateTimer(startTime, false, true);
        timer.Timeout += OnParryWindowStart;
    }

    private void OnParryWindowStart()
    {
        if (!IsInstanceValid(this) || _paused || GetTree().Paused)
            return;

        _inParryWindow = true;

        Color baseColor = _hammerSprite.SelfModulate;
        _parryTween = CreateTween().SetLoops();
        _parryTween.TweenProperty(_hammerSprite, "self_modulate",
            new Color(1.5f, 1.5f, 0.3f), 0.1f);
        _parryTween.TweenProperty(_hammerSprite, "self_modulate",
            baseColor, 0.1f);

        RingProgress = 0f;
        var ringTween = CreateTween().SetLoops();
        ringTween.TweenProperty(this, "RingProgress", 1f, 0.2f);
        ringTween.TweenProperty(this, "RingProgress", 0f, 0.2f);
    }

    public override void _Draw()
    {
        if (!_inParryWindow)
            return;

        float radius = 60f + RingProgress * 40f;
        Color color = new Color(1f, 1f, 0f, 0.8f * (1f - RingProgress));
        DrawArc(Vector2.Zero, radius, 0, Mathf.Tau, 32, color, 3f);
    }

    private void OnMoveButtonClicked(Area2D _)
    {
        if (!IsInstanceValid(_player))
            return;

        foreach (Area2D area in GetOverlappingAreas())
        {
            if (area.GetParent() is Player)
            {
                _dodged = true;
                break;
            }
        }
    }

    private void OnAttackTimerTimeout()
    {
        if (!IsInstanceValid(this) || _paused || GetTree().Paused)
            return;

        Cleanup();

        bool playerInZone = false;

        foreach (Area2D area in GetOverlappingAreas())
        {
            if (area.GetParent() is Player)
            {
                playerInZone = true;
                break;
            }
        }

        if (!_dodged && playerInZone)
        {
            _round?.RegisterHit();
            _eventBus.EmitPlayerGotHit();
        }

        _round?.NotifyHammerFinished(0f, _attackDelay);
        QueueFree();
    }

    private void Cleanup()
    {
        _parryTween?.Kill();
        _inParryWindow = false;

        DisconnectFromEventBus();
    }

    private void DisconnectFromEventBus()
    {
        if (_eventBus == null)
            return;

        if (_eventBus.IsConnected(EventBus.SignalName.MoveButtonClicked,
            new Callable(this, nameof(OnMoveButtonClicked))))
        {
            _eventBus.Disconnect(EventBus.SignalName.MoveButtonClicked,
                new Callable(this, nameof(OnMoveButtonClicked)));
        }

        if (_eventBus.IsConnected(EventBus.SignalName.GamePaused,
            new Callable(this, nameof(OnGamePaused))))
        {
            _eventBus.Disconnect(EventBus.SignalName.GamePaused,
                new Callable(this, nameof(OnGamePaused)));
        }

        if (_eventBus.IsConnected(EventBus.SignalName.GameUnpaused,
            new Callable(this, nameof(OnGameUnpaused))))
        {
            _eventBus.Disconnect(EventBus.SignalName.GameUnpaused,
                new Callable(this, nameof(OnGameUnpaused)));
        }
    }

    public void OnGamePaused()
    {
        _paused = true;
        _parryTween?.Kill();
        _parryTween = null;
        _inParryWindow = false;

        if (_attackTimer != null && !_attackTimer.IsStopped())
        {
            _pausedTimeLeft = _attackTimer.TimeLeft;
            _attackTimer.Stop();
        }
    }

    public void OnGameUnpaused()
    {
        _paused = false;

        if (_attackTimer != null && _pausedTimeLeft > 0f)
        {
            _attackTimer.WaitTime = _pausedTimeLeft;
            _attackTimer.Start();
            _pausedTimeLeft = 0f;
        }
    }

    private void OnHammerAttackZoneEntered(Area2D area)
    {
        if (area.GetParent() is Player)
            _round?.NotifyHammerEntered();
    }

    private void OnHammerAttackZoneExited(Area2D area)
    {
        if (area.GetParent() is Player)
        {
            _round?.NotifyHammerExited();
            _dodged = false;
        }
    }
}
