using Godot;


public partial class EventBus : Node
{
    
    [Signal]
    public delegate void AttackRoundFinishedEventHandler(float reactionTime, bool playerHit);
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


    

    
    public void EmitAttackRoundFinished(float reactionTime, bool playerHit)
    {
        EmitSignal(nameof(AttackRoundFinished), reactionTime, playerHit);
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
