using Godot;

using Vector2 = Godot.Vector2;

public partial class Player : Node2D
{
    [Export] private Timer _hammerSpawnTimer;
    [Export] private Area2D _headAnchor;
    [Export] private Timer _scoreTimer; 
    [Export] private AnimatedSprite2D _playerSprite;    
    
    private float _rotationSpeed;
    private float _roatationAngle = 45f;
    private EventBus _eventBus;
    private bool _isDead = false;
    public bool IsDead => _isDead;
    
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
        
        if (escapeAreaAngle > 0)
        {
            this.RotationDegrees += _roatationAngle;
            if (_playerSprite.Frame < 7 )
            {
                _playerSprite.Frame += 1;
            
            }
            else
            {
                _playerSprite.Frame = 0;
            }
          
        }
        else
        {
            this.RotationDegrees -= _roatationAngle; 
            if (_playerSprite.Frame > 0 )
            {
                _playerSprite.Frame -= 1;

            }
            else
            {
                _playerSprite.Frame = 7;
            }
        }

        

    } 
    
}
