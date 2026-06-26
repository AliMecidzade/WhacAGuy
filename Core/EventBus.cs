using Godot;

public partial class EventBus : Node
{
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