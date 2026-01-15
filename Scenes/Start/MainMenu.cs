using Godot;

public partial class MainMenu : Control
{
	private Button _startButton;
	private Button _settingsButton;
	private Button _quitButton;
	public override void _Ready()
	{
		_startButton = GetNode<Button>("Start");
		_settingsButton = GetNode<Button>("Settings");
		_quitButton = GetNode<Button>("Quit");

		_startButton.Pressed += OnStartButtonPressed;
		_quitButton.Pressed += OnQuitButtonPressed; 
	}


	private void ChangeScene(string sceneName)
	{
		var scene = ResourceLoader.Load<PackedScene>("res://Scenes/"+sceneName + ".tscn");
		GetTree().ChangeSceneToPacked(scene);
	}

	private void OnQuitButtonPressed()
	{
		GetTree().Quit();
	}
	
	private void OnStartButtonPressed()
	{
		ChangeScene("Main/main_scene");
	}
	
	
	
}
