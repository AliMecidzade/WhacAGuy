using System.Collections.Generic;
using System.Linq;
using Godot;
using WhacAGuy.Core;
using WhacAGuy.Gameplay.Controllers;
using WhacAGuy.Gameplay.GameProgression;
using WhacAGuy.Gameplay.Score;
using WhacAGuy.Scenes.DodgeFeedback;

public partial class GameManager : Node2D
{
    [Export] private PackedScene _dodgeFeedbackScene;
    [Export] private Player _player;
    [Export] private HammerSpawner _hammerSpawner;
    [Export] private GameOverMenu _gameOverMenu;
    [Export] private Label _scoreLabel;
    [Export] private Label _roundLabel;
    [Export] private TimerBar _timerBar; 
    
    private bool _isGameOver = false;
    private GameSaveService _gameSaveService;
    private PlayerScore _playerScore;
    private EventBus _eventBus;
    private AttackRoundController _round;
    private RoundProgression _roundProgression;
    public override void _Process(double delta)
    {
        if (_round != null && _round.IsRunning)
            _round.AccumulateTime((float)delta);
    }

    public override void _Ready()
    {
        GD.Print("[GAMEMANAGER] _Ready called");
        GD.Print("[GAMEMANAGER] EventBus exists: " + HasNode("/root/EventBus"));
        
        _eventBus = GetNode<EventBus>("/root/EventBus");
        _eventBus.ResetForNewGame();
    
        _playerScore = new PlayerScore();
        _round = new AttackRoundController();
        _roundProgression = new RoundProgression();
        _gameSaveService = new GameSaveService();
        
        _player.Init(_eventBus);
        _gameOverMenu.Init(_eventBus);
        _hammerSpawner.Init(_eventBus, _player, _round);
        _timerBar.Init(_eventBus);
        _round.Init(_eventBus);
    
        InjectDependenciesInGroup("MoveButtons");

        _player.Start();
        _hammerSpawner.Start();
        _gameOverMenu.Start();
        _timerBar.Start();
        
        _eventBus.Connect(
            EventBus.SignalName.PlayerDied,
            new Callable(this, nameof(OnPlayerDied)));

        _eventBus.Connect(
            EventBus.SignalName.AttackRoundFinished,
            new Callable(this, nameof(OnRoundFinished)));
        StartCurrentRound();
    }

    private void StartCurrentRound()
    {
        
        RoundData roundData = _roundProgression.GetRoundData();
        
        _round.StartRound(roundData.HammerCount, roundData.AttackDelay);
        _hammerSpawner.Configure(roundData);
        
        // _timerBar.ShowTimerBar(roundData.AttackDelay);
    }

    private void OnRoundFinished(
        float dodgeQuality,
        bool playerHit)
    {
        if (_isGameOver)
            return;

        if (playerHit)
            return;

        _playerScore.AddScore(dodgeQuality);

        _scoreLabel.Text =
            $"Score: {_playerScore.Score}";
        
        var rating =
            _playerScore.CalculateRating(dodgeQuality);

        ShowDodgeFeedback(rating);

        _roundProgression.AdvanceRound();

        var roundData =
            _roundProgression.GetRoundData();

        _hammerSpawner.Configure(roundData);
        
        _roundLabel.Text = $"Round: {_roundProgression.CurrentRound}";
        _round.StartRound(roundData.HammerCount, roundData.AttackDelay);
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
        _gameSaveService.SaveOnDeath(_playerScore.Score);
        _eventBus.EmitGameOver();

        _eventBus.Disconnect(
            EventBus.SignalName.AttackRoundFinished,
            new Callable(this, nameof(OnRoundFinished)));

        
        
        GD.Print("GAME OVER");
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