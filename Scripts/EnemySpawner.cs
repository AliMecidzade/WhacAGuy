using Godot;
using System;

public partial class EnemySpawner : Node
{
    
    private EventBus _eventBus;


    public void Init(EventBus eventBus)
    {
        _eventBus = eventBus;
    }
   

    public void Start()
    {
        _eventBus.Connect(nameof(_eventBus.OnEnemySpawnTimerTimeout),
            Callable.From<Vector2>(SpawnEnemyOnPlayer));
        _eventBus.Connect(nameof(_eventBus.OnEnemySpawnTimerTimeout),
            Callable.From<Vector2>(SpawnEnemyRandomly));
    }
    // выяснить почему это говно не работает 14.08.25
    // выяснил 15.08.25

    private Enemy SpawnEnemy()
    {
        var enemyScene = GD.Load<PackedScene>("res://Scenes/enemy.tscn");
        Enemy enemySceneInstance = enemyScene.Instantiate<Enemy>();
        enemySceneInstance.Init(_eventBus);
        return enemySceneInstance; 
    }
    
    private void SpawnEnemyOnPlayer(Vector2 playerPos)
    {
        Enemy newEnemy = SpawnEnemy();
        newEnemy.Position = playerPos;
        AddChild(newEnemy);
        
        
    }

    private void SpawnEnemyRandomly(Vector2 playerPos)
    {

        Enemy newEnemy = SpawnEnemy();
        

    }
    
    
    
    
}
