using Godot;
using System;
using System.Collections.Generic;

public partial class EnemySpawner : Node
{
    [Export] private Timer _enemySpawnTimer;
    
    
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
        foreach (Marker2D child in GetNode("EnemySpawnPoints").GetChildren())
        {
            if (child is Marker2D)
            {
                _spawnPoints.Add(child as Marker2D);
            }
        }
        _enemySpawnTimer.Timeout += EnemySpawnTimerEnded;
    }
    
    private void EnemySpawnTimerEnded()
    {
        _eventBus.EmitSignal(nameof(EventBus.OnEnemySpawnTimerTimeout), _player.PlayerPosition);
        
    }
    public void Start()
    {
        _eventBus.Connect(nameof(_eventBus.OnEnemySpawnTimerTimeout),
            Callable.From<Vector2>(PlaceEnemyAtPlayer));
       
        _eventBus.Connect(nameof(_eventBus.OnEnemySpawnTimerTimeout),
            Callable.From<Vector2>(PlaceEnemyRandomly));
    }
    

    private Enemy CreateEnemy()
    {
        var enemyScene = GD.Load<PackedScene>("res://Scenes/enemy.tscn");
        Enemy enemySceneInstance = enemyScene.Instantiate<Enemy>();
        enemySceneInstance.Init(_eventBus);
        return enemySceneInstance; 
    }
    
    private void PlaceEnemyAtPlayer(Vector2 playerPos)
    {
        Enemy newEnemy = CreateEnemy();
        newEnemy.Position = playerPos;
        AddChild(newEnemy);
        
    }

    private void PlaceEnemyRandomly(Vector2 playerPos)
    {

        int enemyNumber = 3; 
    
        
        List<Node2D> availablePoints = new List<Node2D>(_spawnPoints);
    
        for (int enemyIndex = 0; enemyIndex < enemyNumber; enemyIndex++)
        {
            if (availablePoints.Count == 0)
            {
                GD.Print("Нет больше точек для спавна!");
                break;
            }

            int positionIndex = _random.Next(0, availablePoints.Count);

            
            while (availablePoints[positionIndex].Position == playerPos)
            {
                availablePoints.RemoveAt(positionIndex);

                if (availablePoints.Count == 0)
                {
                    GD.Print("Остались только точки на позиции игрока!");
                    return;
                }

                positionIndex = _random.Next(0, availablePoints.Count);
            }

            Enemy newEnemy = CreateEnemy();
            newEnemy.Position = availablePoints[positionIndex].Position;
            AddChild(newEnemy);

            
            availablePoints.RemoveAt(positionIndex);
        }
        
        
        
    }
    
    
    
    
}
