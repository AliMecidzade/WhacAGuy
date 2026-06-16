using Godot;
using System;
using System.Collections.Generic;
using WhacAGuy.Gameplay.Controllers;
using WhacAGuy.Gameplay.GameProgression;

public partial class HammerSpawner : Node
{
    [Export] private Timer _hammerSpawnTimer;

    private EventBus _eventBus;
    private Player _player;
    private AttackRoundController _round;

    private readonly List<Node2D> _spawnPoints = new();
    private readonly Random _random = new();

    private Callable _placeHammersCallable;

    private RoundData _currentRoundData = new()
    {
        HammerCount = 4,
        SpawnDelay = 2f,
        AttackDelay = 0.5f
    };

    public void Init(EventBus eventBus, Player player, AttackRoundController round)
    {
        _eventBus = eventBus;
        _player = player;
        _round = round;
    }

    public override void _Ready()
    {
        foreach (Marker2D child in GetNode("HammerSpawnPoints").GetChildren())
            _spawnPoints.Add(child);

        _hammerSpawnTimer.Timeout += HammerSpawnTimerEnded;
    }

    public void Start()
    {
        _placeHammersCallable = Callable.From<Vector2, int>(PlaceHammers);

        if (!_eventBus.IsConnected(EventBus.SignalName.OnHammerSpawnTimerTimeout, _placeHammersCallable))
        {
            _eventBus.Connect(
                EventBus.SignalName.OnHammerSpawnTimerTimeout,
                _placeHammersCallable);
        }

        var stopCallable = new Callable(this, nameof(Stop));
        if (!_eventBus.IsConnected(EventBus.SignalName.GameOver, stopCallable))
        {
            _eventBus.Connect(
                EventBus.SignalName.GameOver,
                stopCallable);
        }
    }

    public void Configure(RoundData roundData)
    {
        _currentRoundData = roundData;

        _hammerSpawnTimer.Stop();
        _hammerSpawnTimer.WaitTime = Mathf.Max(0.05f, _currentRoundData.SpawnDelay);
        _hammerSpawnTimer.Start();
    }

    private void HammerSpawnTimerEnded()
    {
        _eventBus.EmitEnemySpawn(_player.PlayerPosition, _currentRoundData.HammerCount);
    }

    public void Stop()
    {
        _hammerSpawnTimer.Stop();

        if (_eventBus.IsConnected(EventBus.SignalName.OnHammerSpawnTimerTimeout, _placeHammersCallable))
            _eventBus.Disconnect(EventBus.SignalName.OnHammerSpawnTimerTimeout, _placeHammersCallable);

        var stopCallable = new Callable(this, nameof(Stop));
        if (_eventBus.IsConnected(EventBus.SignalName.GameOver, stopCallable))
            _eventBus.Disconnect(EventBus.SignalName.GameOver, stopCallable);
    }

    private Hammer CreateHammer()
    {
        var hammerScene = GD.Load<PackedScene>("res://Gameplay/Hammer/hammer.tscn");
        Hammer hammer = hammerScene.Instantiate<Hammer>();

        hammer.Init(_eventBus, _round);
        hammer.Configure(_currentRoundData.AttackDelay);

        return hammer;
    }

    private void PlaceHammers(Vector2 playerPos, int hammerNumber)
    {
        if (hammerNumber <= 0)
            return;

        Hammer centerHammer = CreateHammer();
        AddChild(centerHammer);
        centerHammer.GlobalPosition = playerPos;

        List<Node2D> availablePoints = new(_spawnPoints);
        availablePoints.RemoveAll(p => p.GlobalPosition.DistanceTo(playerPos) < 30f);

        for (int i = 1; i < hammerNumber; i++)
        {
            if (availablePoints.Count == 0)
                break;

            int index = _random.Next(availablePoints.Count);
            Vector2 spawnGlobalPos = availablePoints[index].GlobalPosition;

            Hammer hammer = CreateHammer();
            AddChild(hammer);
            hammer.GlobalPosition = spawnGlobalPos;

            availablePoints.RemoveAt(index);
        }
    }
}