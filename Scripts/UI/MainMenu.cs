using Godot;
using System;

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

	}


	private void ChangeScene(string SceneName)
	{
		
	}
	
	
	private void OnStartButtonPressed()
	{
		
	}
	
	
	
}
