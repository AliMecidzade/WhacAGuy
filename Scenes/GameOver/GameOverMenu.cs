using Godot;
using System;

public partial class GameOverMenu : CanvasLayer
{
	
	private EventBus _eventBus;

	[Export] private Button _tryAgainButton; 
	[Export] private Button _exitButton;

	public void Init(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void Start()
	{
		_eventBus.Connect(EventBus.SignalName.PlayerGotHit, 
			new Callable(this, nameof(ShowGameOverMenu)));
	}
	
	
	public override void _Ready()
	{

		_exitButton.Pressed += OnExitButtonPressed;

		_tryAgainButton.Pressed += OnTryAgainButtonPressed;
	}


	private void OnExitButtonPressed()
	{
		GetTree().Quit();
	}

	private void OnTryAgainButtonPressed()
	{
		GetTree().ReloadCurrentScene();
	}

	private void ShowGameOverMenu()
	{
		this.Visible = true;
	}
	
	
	
}
