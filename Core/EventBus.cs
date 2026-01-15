using Godot;


public partial class EventBus : Node
{
    
    
    [Signal]
    public delegate void GameOverEventHandler();
    [Signal]
    public delegate void MoveButtonClickedEventHandler(Area2D hoveredArea);
    
    [Signal]
    public delegate void OnHammerSpawnTimerTimeoutEventHandler(Vector2 position, int hammerCount);
   
    [Signal]
    public delegate void PlayerGotHitEventHandler();





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
