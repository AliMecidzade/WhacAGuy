using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class GameManager : Node2D
{
    [Export]
    private Player _player;

    [Export] private EnemySpawner _enemySpawner; 
    
    [Export] private GameOverMenu _gameOverMenu;
    
    private EventBus _eventBus;
    
    
    public override void _Ready()
    {
        
        _eventBus = new EventBus();
        _player.Init(_eventBus);
        _enemySpawner.Init(_eventBus , _player);
        _gameOverMenu.Init(_eventBus);
        _player.Start();
        _enemySpawner.Start();
        _gameOverMenu.Start();
        InjectDependenciesInGroup("MoveButtons"); 



    }
    
    private void InjectDependenciesInGroup(string groupName)
    {
        foreach (var node in GetTree().GetNodesInGroup(groupName))
        {
            if (node is IEventBusInjectable injectable)
                injectable.Init(_eventBus);
        }
    }

    
    
}
