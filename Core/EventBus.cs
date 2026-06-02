using Godot;


public partial class EventBus : Node
{
    
    [Signal]
    public delegate void AttackRoundFinishedEventHandler(float reactionTime, bool playerHit,float maxTime);
    [Signal]
    public delegate void GameOverEventHandler();
    [Signal]
    public delegate void MoveButtonClickedEventHandler(Area2D hoveredArea);
    
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
        DisconnectAll(SignalName.AttackRoundFinished);
        DisconnectAll(SignalName.GameOver);
        DisconnectAll(SignalName.MoveButtonClicked);
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

    
    public void EmitAttackRoundFinished(float reactionTime, bool playerHit, float maxTime)
    {
        EmitSignal(nameof(AttackRoundFinished), reactionTime, playerHit, maxTime);
    }
    public void EmitHammerDodged(float timeInside)
    {
        EmitSignal(nameof(HammerDodged), timeInside);
    }
    public void EmitPlayerDied()
    {
        EmitSignal(nameof(PlayerDied));
    }
    public void EmitGameOver()
    {
        EmitSignal(nameof(GameOver));
    }
    public void EmitEnemySpawn(Vector2 playerPos, int hammerCount)
    {
        GD.Print($"[EVENTBUS] EmitEnemySpawn {playerPos}");

        EmitSignal(nameof(OnHammerSpawnTimerTimeout), playerPos, hammerCount);
    }
     
    public void EmitPlayerGotHit()
    {
        EmitSignal(nameof(PlayerGotHit));
    }
    public void EmitMoveButtonClicked(Area2D area)
    {
        EmitSignal(nameof(MoveButtonClicked), area);
    } 
    

}
