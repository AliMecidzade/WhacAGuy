using Godot;
using WhacAGuy.Gameplay.Controllers;
using WhacAGuy.Scenes.DodgeFeedback;


public partial class GameManager : Node2D
{
    [Export] private PackedScene _dodgeFeedbackScene;

    [Export] private Player _player;
    [Export] private HammerSpawner _hammerSpawner;
    [Export] private GameOverMenu _gameOverMenu;
    [Export] private Label _scoreLabel;

    private bool _isGameOver = false;
    private PlayerScore _playerScore;

    private EventBus _eventBus;
    private AttackRoundController _round;

    public override void _Ready()
    {
        GD.Print("[GAMEMANAGER] _Ready called");
        GD.Print("[GAMEMANAGER] EventBus exists: " + HasNode("/root/EventBus"));

        _eventBus = GetNode<EventBus>("/root/EventBus");
        _eventBus.ResetForNewGame();

        _playerScore = new PlayerScore();
        _round = new AttackRoundController();

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

    }

    private void OnRoundFinished(float timeInside, bool playerHit, float maxTime)
    {
        if (_isGameOver)
            return;

        if (playerHit)
            return;

        _playerScore.AddScore(timeInside, playerHit, maxTime);
        _scoreLabel.Text = $"Score: {_playerScore.Score}";

        var rating = _playerScore.CalculateRating(timeInside, playerHit, maxTime);
        ShowDodgeFeedback(rating);

    }

    private void ShowDodgeFeedback(DodgeRating rating)
    {
        var feedback = _dodgeFeedbackScene.Instantiate<DodgeFeedback>();
        AddChild(feedback);
        feedback.Position = new Vector2(100, 100);
        feedback.Show(rating);
    }

    private void OnPlayerDied()
    {
        if (_isGameOver)
            return;

        _isGameOver = true;
        _eventBus.EmitGameOver();

        _eventBus.Disconnect(
            EventBus.SignalName.AttackRoundFinished,
            new Callable(this, nameof(OnRoundFinished)));
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