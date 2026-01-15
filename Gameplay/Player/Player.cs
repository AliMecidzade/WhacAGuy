using Godot;

using Vector2 = Godot.Vector2;

public partial class Player : Node2D
{
    [Export] private Timer _hammerSpawnTimer;
    [Export] private Area2D _headAnchor;
    [Export] private Timer _scoreTimer; 
    
    
    private float _rotationSpeed;
    private float _roatationAngle = 45f;
    private EventBus _eventBus;
    private PlayerScore _playerScore;
    private bool _isDead;
    
    
    public void Init(EventBus eventBus)
    {
        _eventBus = eventBus;
        
    }

    
    
    public override void _Ready()
    {
       
       GD.Print("Player Ready");

       _playerScore = GetNode<PlayerScore>("PlayerScore");

       if (_playerScore != null)
       {
           GD.Print("PlayerScore Ready");
       }
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
        
        _eventBus.Connect( EventBus.SignalName.MoveButtonClicked, 
            new Callable(this, nameof(OnMoveButtonClicked)));
        
        _eventBus.Connect(EventBus.SignalName.PlayerGotHit, 
            new Callable(this, nameof(Die)));

        // _eventBus.Connect(EventBus.SignalName.OnHammerSpawnTimerTimeout,
        //     new Callable(this, nameof(OnDodgeTimerStart)));
    }



    private void OnDodgeTimerStart()
    {
        
        
    }
    
    private void OnMoveButtonClicked(Area2D area) 
    {
        
        float rotationDegrees = area.RotationDegrees;
        
        Rotate(rotationDegrees);
    }

    private void Die()
    {
        _isDead = true;
        QueueFree();
        
    }
    
    
    private void Rotate(float escapeAreaAngle)
    {
        
        if (escapeAreaAngle > 0)
        {
            this.RotationDegrees += _roatationAngle; 
            GD.Print(this.RotationDegrees);

        }
        else
        {
            this.RotationDegrees -= _roatationAngle; 
            GD.Print(this.RotationDegrees);
        }
        
        
    } 
    
}
