using Godot;
using System;
using System.Data;

public partial class GameOverMenu : CanvasLayer
{
	
	private EventBus _eventBus;

	[Export] private Button _tryAgainButton; 
	[Export] private Button _exitButton;
	[Export] private Button _backToMainMenuButton;
	[Export] private Label _playerHighScore;
	 
	public void Init(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void Start()
	{
		if (_eventBus == null)
		{
			GD.PrintErr("GameOverMenu: EventBus is NULL");
			return;
		}

		_eventBus.Connect(
			EventBus.SignalName.GameOver,
			new Callable(this, nameof(ShowGameOverMenu))
		);
	}
	
	public override void _Ready()
	{
		
		_exitButton.Pressed += OnExitButtonPressed;
		_backToMainMenuButton.Pressed += OnBackToMainButtonPressed;
		_tryAgainButton.Pressed += OnTryAgainButtonPressed;
	}

	private void ChangeScene(string sceneName)
	{
		var scene = ResourceLoader.Load<PackedScene>("res://Scenes/"+sceneName + ".tscn");
		GetTree().ChangeSceneToPacked(scene);
	}
	
	
	private void OnBackToMainButtonPressed()
	{
		ChangeScene("Start/start_scene");
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
		SaveData data = SaveManager.Instance.Load();
		_playerHighScore.Text = "High Score: " + data.HighScore.ToString();
		this.Visible = true;
	}
	
	
	
}
