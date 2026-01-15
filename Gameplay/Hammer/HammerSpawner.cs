using Godot;
using System;
using System.Collections.Generic;

public partial class HammerSpawner : Node
{
    [Export] private Timer _hammerSpawnTimer;
    
    
    private EventBus _eventBus;
    private List<Node2D> _spawnPoints = new List<Node2D>();
    private Random _random = new Random();
    private Player _player;
    
    public void Init(EventBus eventBus, Player player)
    {
       
        _eventBus = eventBus;
        _player = player;
    }


    public override void _Ready()
    {
        foreach (Marker2D child in GetNode("HammerSpawnPoints").GetChildren())
        {
            if (child is Marker2D)
            {
                _spawnPoints.Add(child as Marker2D);
            }
        }
        _hammerSpawnTimer.Timeout += HammerSpawnTimerEnded;
    }
    
    private void HammerSpawnTimerEnded()
    {
        _eventBus.EmitSignal(nameof(EventBus.OnHammerSpawnTimerTimeout), _player.PlayerPosition , 4);
        
    }
    public void Start()
    {
        _eventBus.Connect(nameof(EventBus.OnHammerSpawnTimerTimeout), 
            Callable.From<Vector2, int>(PlaceHammers)); 
    }
    
    
    
    
    private Hammer CreateHammer() 
    { 
        var hammerScene = GD.Load<PackedScene>("res://Gameplay/Hammer/hammer.tscn");
        Hammer hammerSceneInstance = hammerScene.Instantiate<Hammer>(); 
        hammerSceneInstance.Init(_eventBus); 
        return hammerSceneInstance;
        
    }
    private void PlaceHammers(Vector2 playerPos, int hammerNumber)
    {
        
        Hammer hammerOnPlayer = CreateHammer();
        hammerOnPlayer.Position = playerPos;
        AddChild(hammerOnPlayer);

      
        List<Node2D> availablePoints = new List<Node2D>(_spawnPoints);
        availablePoints.RemoveAll(p => p.Position == playerPos);

        for (int i = 0; i < hammerNumber; i++)
        {
            if (availablePoints.Count == 0)
                break;

            int index = _random.Next(availablePoints.Count);

            Hammer hammer = CreateHammer();
            hammer.Position = availablePoints[index].Position;
            AddChild(hammer);

            availablePoints.RemoveAt(index);
        }
    }

        
        
        
    }
    
    
    
    
