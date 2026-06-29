using Godot;
using System.Collections.Generic;
using WhacAGuy.Gameplay.Audio;

public partial class AudioHandler : Node, IEventBusInjectable
{
    private AudioStreamPlayer _sfxPlayer;
    private Dictionary<SoundId, AudioStream> _sounds = new Dictionary<SoundId, AudioStream>();
    private EventBus _eventBus;

    public void Init(EventBus eventBus)
    {
        _eventBus = eventBus;

        _sfxPlayer = new AudioStreamPlayer();
        AddChild(_sfxPlayer);

        LoadSounds();
        ConnectSignals();
    }

    private void LoadSounds()
    {
        _sounds[SoundId.ButtonPressed] = GD.Load<AudioStream>("res://Assets/Audio/Buttons/floraphonic-analog-appliance-button-10-185285.mp3");
    }

    private void ConnectSignals()
    {
        _eventBus.Connect(EventBus.SignalName.MoveButtonClicked,
            new Callable(this, nameof(OnMoveButtonPressed)));
    }

    private void OnMoveButtonPressed(Area2D _)
    {
        PlaySound(SoundId.ButtonPressed);
    }

    private void PlaySound(SoundId soundId)
    {
        _sfxPlayer.Stream = _sounds[soundId];
        _sfxPlayer.Play();
    }
}
