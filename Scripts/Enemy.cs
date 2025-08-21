using Godot;
using System;

public partial class Enemy : Area2D, IEventBusInjectable
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
		
		this.BodyEntered += OnEnemyAttackZoneEntered;
		this.BodyExited += OnEnemyAttackZoneExited; 
	}

	
	private void OnAttackTimerTimeout()
	{

		if (_isPlayerInArea)
		{
			_eventBus.EmitSignal(nameof(EventBus.PlayerGotHit));
		}
		QueueFree();
	}

	private void OnEnemyAttackZoneEntered(Node playerNode)
	{
		_isPlayerInArea = true; 
		
	}

	private void OnEnemyAttackZoneExited(Node playerNode)
	{
		_isPlayerInArea = false; 
		
	}

	
}
