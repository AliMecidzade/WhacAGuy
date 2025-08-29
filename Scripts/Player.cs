using Godot;
using System;
using System.Diagnostics;
using System.Numerics;
using Godot.NativeInterop;
using Vector2 = Godot.Vector2;

public partial class Player : Node2D
{
    [Export] private Timer _enemySpawnTimer;
    [Export] private CollisionShape2D _playerCollision; 
    
    private float _rotationSpeed;
    private float _roatationAngle = 45f;
    private Godot.Vector2 _playerPosition; 
    private EventBus _eventBus;
    
    
    public void Init(EventBus eventBus)
    {
        _eventBus = eventBus; 
        
    }

    
    
    public override void _Ready()
    {
       
       GD.Print("Player Ready");


      
    }

    public Vector2 PlayerPosition => _playerCollision.GlobalPosition;
    
    public void Start()
    {
        _eventBus.Connect( EventBus.SignalName.MoveButtonClicked, 
            new Callable(this, nameof(OnMoveButtonClicked)));
        
        _eventBus.Connect(EventBus.SignalName.PlayerGotHit, 
            new Callable(this, nameof(Die)));
    }
    
    
    private void OnMoveButtonClicked(Area2D area) 
    {
        float rotationDegrees = area.RotationDegrees;
        
        Rotate(rotationDegrees);
    }

    private void Die()
    {
        _eventBus.EmitPlayerGotHit();
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
