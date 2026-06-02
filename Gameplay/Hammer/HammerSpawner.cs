using Godot;
using System;
using System.Collections.Generic;
using WhacAGuy.Gameplay.Controllers;

public partial class HammerSpawner : Node
{
    [Export] private Timer _hammerSpawnTimer;
    [Export] private int _hammerCount = 0;

    private EventBus _eventBus;

    private readonly List<Node2D> _spawnPoints = new();
    private readonly Random _random = new();

    private Player _player;
    private AttackRoundController _round;

    private Callable _placeHammersCallable;

    public void Init(EventBus eventBus, Player player, AttackRoundController round)
    {
        _eventBus = eventBus;
        _player = player;
        _round = round;
    }

    public override void _Ready()
    {
        foreach (Marker2D child in GetNode("HammerSpawnPoints").GetChildren())
        {
            _spawnPoints.Add(child);
        }

        _hammerSpawnTimer.Timeout += HammerSpawnTimerEnded;
    }

    public void Start()
    {
        _placeHammersCallable = Callable.From<Vector2, int>(PlaceHammers);

        if (!_eventBus.IsConnected(nameof(EventBus.OnHammerSpawnTimerTimeout), _placeHammersCallable))
        {
            _eventBus.Connect(
                nameof(EventBus.OnHammerSpawnTimerTimeout),
                _placeHammersCallable
            );
        }

        var stopCallable = new Callable(this, nameof(Stop));
        if (!_eventBus.IsConnected(nameof(EventBus.GameOver), stopCallable))
        {
            _eventBus.Connect(
                nameof(EventBus.GameOver),
                stopCallable
            );
        }

        _hammerSpawnTimer.Start();
    }

    private void HammerSpawnTimerEnded()
    {
        _eventBus.EmitEnemySpawn(_player.PlayerPosition, _hammerCount);
    }

    public void Stop()
    {
        _hammerSpawnTimer.Stop();

        if (_eventBus.IsConnected(nameof(EventBus.OnHammerSpawnTimerTimeout), _placeHammersCallable))
            _eventBus.Disconnect(nameof(EventBus.OnHammerSpawnTimerTimeout), _placeHammersCallable);

        var stopCallable = new Callable(this, nameof(Stop));
        if (_eventBus.IsConnected(nameof(EventBus.GameOver), stopCallable))
            _eventBus.Disconnect(nameof(EventBus.GameOver), stopCallable);
    }

    private Hammer CreateHammer()
    {
        var hammerScene = GD.Load<PackedScene>("res://Gameplay/Hammer/hammer.tscn");
        Hammer hammer = hammerScene.Instantiate<Hammer>();
        hammer.Init(_eventBus, _round);
        return hammer;
    }

    private void PlaceHammers(Vector2 playerPos, int hammerNumber)
    {
        int totalHammers = 1;

        Hammer scoringHammer = CreateHammer();
        scoringHammer.SetAsScoringHammer();

        List<Node2D> availablePoints = new(_spawnPoints);
        availablePoints.RemoveAll(p => p.GlobalPosition.DistanceTo(playerPos) < 30f);

        List<(Hammer hammer, Vector2 pos)> regularHammers = new();
        for (int i = 0; i < hammerNumber; i++)
        {
            if (availablePoints.Count == 0)
                break;

            int index = _random.Next(availablePoints.Count);
            Vector2 spawnPos = availablePoints[index].GlobalPosition;
            availablePoints.RemoveAt(index);

            regularHammers.Add((CreateHammer(), spawnPos));
            totalHammers++;
        }

        _round.StartRound(totalHammers);

        AddChild(scoringHammer);
        scoringHammer.GlobalPosition = playerPos;

        foreach (var (hammer, pos) in regularHammers)
        {
            AddChild(hammer);
            hammer.GlobalPosition = pos;
        }
    }
}