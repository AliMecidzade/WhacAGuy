using Godot;
using WhacAGuy.Gameplay.Controllers;


public partial class GameManager : Node2D
{
    
    [Export] private Player _player;

    [Export] private HammerSpawner _hammerSpawner; 
    
    [Export] private GameOverMenu _gameOverMenu;

    [Export] private Label _scoreLabel; 
    
    private PlayerScore _playerScore;
    
    private EventBus _eventBus;

    private AttackRoundController _round; 
    
    public override void _Ready()
    {
        _eventBus = new EventBus();
        AddChild(_eventBus);

        _playerScore = new PlayerScore();
        _round = new AttackRoundController();
        AddChild(_round);

        _player.Init(_eventBus);
        _gameOverMenu.Init(_eventBus);
        _hammerSpawner.Init(_eventBus, _player, _round);

        _round.Init(_eventBus);

        InjectDependenciesInGroup("MoveButtons");

        _player.Start();
        _hammerSpawner.Start();
        _gameOverMenu.Start();

        _eventBus.Connect(
            EventBus.SignalName.PlayerDied,
            new Callable(this, nameof(OnPlayerDied))
        );

        _eventBus.Connect(
            EventBus.SignalName.AttackRoundFinished,
            new Callable(this, nameof(OnRoundFinished))
        );

        _round.StartRound();
    }
    
    private void OnRoundFinished(float reactionTime, bool playerHit)
    {
        if (_player.IsDead)
            return;

        int roundScore =
            _playerScore.CalculateScore(reactionTime, playerHit);

        _playerScore.AddScore(reactionTime, playerHit);

        GD.Print("REACTION TIME: " + reactionTime);
        GD.Print("ROUND SCORE: " + roundScore);
        GD.Print("TOTAL SCORE: " + _playerScore.Score);
        _scoreLabel.Text = "Score: " + _playerScore.Score.ToString();

        _round.StartRound();
    }
    
    private void OnPlayerDied()
    {
        _eventBus.EmitGameOver();
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
