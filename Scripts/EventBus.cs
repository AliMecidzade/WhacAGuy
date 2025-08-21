using Godot;
using System;

public partial class EventBus : Node
{
    
    [Signal]
    public delegate void MoveButtonClickedEventHandler(Area2D hoveredArea);
    
    
    [Signal]
    public delegate void OnEnemySpawnTimerTimeoutEventHandler(Vector2 position);
   
   [Signal]
   public delegate void PlayerGotHitEventHandler();



 
    public void EmitMoveButtonClicked(Area2D area)
    {
        EmitSignal(nameof(MoveButtonClicked), area);
    } 
    

}
