using Godot;

using Vector2 = Godot.Vector2;

public partial class Player : Node2D
{
    [Export] private Timer _hammerSpawnTimer;
    [Export] private Area2D _headAnchor;
    [Export] private Timer _scoreTimer; 
    [Export] private AnimatedSprite2D _playerSprite;    
    
    private float _rotationAngle = 45f;
    private EventBus _eventBus;
    private bool _isDead = false;
    private bool _isRotating = false;
    private Tween _tween;
    public bool IsDead => _isDead;
    public bool IsMoving => _isRotating;

    private void OnRotationFinished()
    {
        _isRotating = false;
        _playerSprite.SpeedScale = 1.0f;
        _playerSprite.Stop();
    }
    
    public void Init(EventBus eventBus)
    {
        _eventBus = eventBus;
    }
    
    
    public override void _Ready()
    { 
       GD.Print("Player Ready");

    }

    
    public Vector2 PlayerPosition
    {
        get
        {
            if (!IsInstanceValid(_headAnchor))
                return Vector2.Zero;

            return _headAnchor.GlobalPosition;
        }
    }

    public void Start()
    {
        
        _eventBus.Connect(EventBus.SignalName.MoveButtonClicked, 
            new Callable(this, nameof(OnMoveButtonClicked)));
        
        _eventBus.Connect(EventBus.SignalName.PlayerGotHit, 
            new Callable(this, nameof(TakeDamage)));

    }


    private void TakeDamage()
    {
        if (_isDead)
            return;

        _isDead = true;
        _eventBus.EmitPlayerDied();
        GD.Print("PLAYER DIED");
        QueueFree();
      
    }
    
    
    
    private void OnMoveButtonClicked(Area2D area) 
    {
        
        float rotationDegrees = area.RotationDegrees;
        
        Rotate(rotationDegrees);
        
        
    }

   
    
     private new void Rotate(float escapeAreaAngle)
    {
        if (_isRotating) return;
        _isRotating = true;

        _tween = CreateTween();
        _tween.Finished += OnRotationFinished;
        _tween.SetTrans(Tween.TransitionType.Sine);

        _playerSprite.SpeedScale = 4.0f;

        if (escapeAreaAngle > 0)
        {
            _playerSprite.Play("spin");
            _tween.TweenProperty(this, "rotation", Rotation + Mathf.DegToRad(_rotationAngle), 0.22f);
        }
        else
        {
            _playerSprite.PlayBackwards("spin");
            _tween.TweenProperty(this, "rotation", Rotation - Mathf.DegToRad(_rotationAngle), 0.22f);
        }
    } 
    
}
