using Godot;
using System;

public partial class Hammer : Area2D, IEventBusInjectable
{
	
	private Timer _attackTimer;
	
	
	private EventBus _eventBus;
	
	private bool _isPlayerInArea = false;

	
	public void Init(EventBus eventBus)
	{
		_eventBus = eventBus;
	}
	
	public override void _Ready()
	{
		_attackTimer = GetNode<Timer>("BeforeAttackTimer");
		
		
		_attackTimer.Timeout += OnAttackTimerTimeout;
		
		this.AreaEntered += OnHammerAttackZoneEntered;
		this.AreaExited += OnHammerAttackZoneExited; 
	}

	
	private void OnAttackTimerTimeout()
	{
		GD.Print(_isPlayerInArea);
		if (_isPlayerInArea)
		{
			_eventBus.EmitSignal(nameof(EventBus.PlayerGotHit));
			
		}
		QueueFree();
	}

	private void OnHammerAttackZoneEntered(Area2D playerHead)
	{
		_isPlayerInArea = true; 
		
	}

	private void OnHammerAttackZoneExited(Area2D playerHead)
	{
		_isPlayerInArea = false; 
		
	}

	
}
