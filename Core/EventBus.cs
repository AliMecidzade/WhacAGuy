using Godot;

public partial class EventBus : Node
{
    private bool _userPaused = false;
    private CanvasLayer _pauseOverlay;

    public override void _Ready()
    {
        ProcessMode = Node.ProcessModeEnum.Always;
        CreatePauseOverlay();
        Connect(SignalName.GameOver, new Callable(this, nameof(OnGameOverFromBus)));
    }

    public bool IsPaused => _userPaused;

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("pause"))
        {
            _userPaused = !_userPaused;
            GetTree().Paused = _userPaused;
            if (_userPaused)
                EmitGamePaused();
            else
                EmitGameUnpaused();
            if (_pauseOverlay != null)
                _pauseOverlay.Visible = _userPaused;
        }
    }

    private void CreatePauseOverlay()
    {
        _pauseOverlay = new CanvasLayer();
        _pauseOverlay.Layer = 128;
        _pauseOverlay.Visible = false;

        var bg = new ColorRect();
        bg.Color = new Color(0, 0, 0, 0.6f);
        bg.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        bg.MouseFilter = Control.MouseFilterEnum.Ignore;
        _pauseOverlay.AddChild(bg);

        var center = new CenterContainer();
        center.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        _pauseOverlay.AddChild(center);

        var vbox = new VBoxContainer();
        vbox.AddThemeConstantOverride("separation", 30);
        center.AddChild(vbox);

        var pausedLabel = new Label();
        pausedLabel.Text = "PAUSED";
        pausedLabel.HorizontalAlignment = HorizontalAlignment.Center;
        pausedLabel.AddThemeFontSizeOverride("font_size", 72);
        pausedLabel.AddThemeColorOverride("font_color", new Color(1, 1, 1));
        vbox.AddChild(pausedLabel);

        var hintLabel = new Label();
        hintLabel.Text = "Press Escape to unpause";
        hintLabel.HorizontalAlignment = HorizontalAlignment.Center;
        hintLabel.AddThemeFontSizeOverride("font_size", 28);
        hintLabel.AddThemeColorOverride("font_color", new Color(0.8f, 0.8f, 0.8f));
        vbox.AddChild(hintLabel);

        AddChild(_pauseOverlay);
    }

    private void OnGameOverFromBus()
    {
        _userPaused = false;
        if (_pauseOverlay != null)
            _pauseOverlay.Visible = false;
    }

    [Signal]
    public delegate void GamePausedEventHandler();

    [Signal]
    public delegate void GameUnpausedEventHandler();

    [Signal]
    public delegate void AttackRoundFinishedEventHandler(
        float dodgeQuality,
        bool playerHit);
    [Signal]
    public delegate void GameOverEventHandler();

    [Signal]
    public delegate void RoundAttackPhaseStartedEventHandler(float attackDelay); 
    
    [Signal]
    public delegate void MoveButtonClickedEventHandler(Area2D hoveredArea);

    [Signal]
    public delegate void MoveClickedEventHandler(bool isLeft);

    [Signal]
    public delegate void OnHammerSpawnTimerTimeoutEventHandler(Vector2 position, int hammerCount);

    [Signal]
    public delegate void PlayerGotHitEventHandler();

    [Signal]
    public delegate void PlayerDiedEventHandler();

    [Signal]
    public delegate void HammerDodgedEventHandler(float timeInside);

    public void ResetForNewGame()
    {
        DisconnectAll(SignalName.RoundAttackPhaseStarted);
        DisconnectAll(SignalName.AttackRoundFinished);
        DisconnectAll(SignalName.GamePaused);
        DisconnectAll(SignalName.GameUnpaused);
        DisconnectAll(SignalName.GameOver);
        DisconnectAll(SignalName.MoveButtonClicked);
        DisconnectAll(SignalName.MoveClicked);
        DisconnectAll(SignalName.OnHammerSpawnTimerTimeout);
        DisconnectAll(SignalName.PlayerGotHit);
        DisconnectAll(SignalName.PlayerDied);
        DisconnectAll(SignalName.HammerDodged);
    }

    private void DisconnectAll(StringName signalName)
    {
        foreach (var connection in GetSignalConnectionList(signalName))
        {
            var callable = (Callable)connection["callable"];
            if (IsConnected(signalName, callable))
                Disconnect(signalName, callable);
        }
    }

    public void EmitGamePaused()
    {
        EmitSignal(SignalName.GamePaused);
    }

    public void EmitGameUnpaused()
    {
        EmitSignal(SignalName.GameUnpaused);
    }

    public void EmitRoundAttackPhaseStarted(
        float attackDelay)
    {
        EmitSignal(
            nameof(RoundAttackPhaseStarted), 
            attackDelay);
    }
    public void EmitAttackRoundFinished(
        float dodgeQuality,
        bool playerHit)
    {
        EmitSignal(
            nameof(AttackRoundFinished),
            dodgeQuality,
            playerHit);
    }
    
    public void EmitHammerDodged(float timeInside)
    {
        EmitSignal(SignalName.HammerDodged, timeInside);
    }

    public void EmitPlayerDied()
    {
        EmitSignal(SignalName.PlayerDied);
    }

    public void EmitGameOver()
    {
        EmitSignal(SignalName.GameOver);
    }

    public void EmitEnemySpawn(Vector2 playerPos, int hammerCount)
    {
        GD.Print($"[EVENTBUS] EmitEnemySpawn {playerPos}");
        EmitSignal(SignalName.OnHammerSpawnTimerTimeout, playerPos, hammerCount);
    }

    public void EmitPlayerGotHit()
    {
        EmitSignal(SignalName.PlayerGotHit);
    }

    public void EmitMoveButtonClicked(Area2D area)
    {
        EmitSignal(SignalName.MoveButtonClicked, area);
    }

    public void EmitMoveClicked(bool isLeft)
    {
        EmitSignal(SignalName.MoveClicked, isLeft);
    }
}