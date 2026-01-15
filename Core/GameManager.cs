using Godot;


public partial class GameManager : Node2D
{
    
    [Export] private Player _player;

    [Export] private HammerSpawner _hammerSpawner; 
    
    [Export] private GameOverMenu _gameOverMenu;
    
    private EventBus _eventBus;
    
    
    public override void _Ready()
    {
        
        _eventBus = new EventBus();
        this.AddChild(_eventBus);
        _player.Init(_eventBus);
        _hammerSpawner.Init(_eventBus , _player);
        _gameOverMenu.Init(_eventBus);
        _player.Start();
        _hammerSpawner.Start();
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
