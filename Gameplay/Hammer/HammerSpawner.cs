using Godot;
using System;
using System.Collections.Generic;
using WhacAGuy.Gameplay.Controllers;

public partial class HammerSpawner : Node
{
    [Export] private Timer _hammerSpawnTimer;

    private EventBus _eventBus;

    private readonly List<Node2D> _spawnPoints = new();
    private readonly Random _random = new();

    private Player _player;
    private AttackRoundController _round;

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
        _eventBus.Connect(
            nameof(EventBus.OnHammerSpawnTimerTimeout),
            Callable.From<Vector2, int>(PlaceHammers)
        );

        _eventBus.Connect(
            nameof(EventBus.GameOver),
            new Callable(this, nameof(Stop))
        );
    }

    private void HammerSpawnTimerEnded()
    {
        _eventBus.EmitEnemySpawn(_player.PlayerPosition, 4);
    }

    private void Stop()
    {
        _hammerSpawnTimer.Stop();
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
        Hammer scoringHammer = CreateHammer();

        scoringHammer.SetAsScoringHammer();
        scoringHammer.Position = playerPos;

        AddChild(scoringHammer);

        List<Node2D> availablePoints = new(_spawnPoints);

        availablePoints.RemoveAll(p => p.Position == playerPos);

        for (int i = 0; i < hammerNumber; i++)
        {
            if (availablePoints.Count == 0)
                break;

            int index = _random.Next(availablePoints.Count);

            Hammer hammer = CreateHammer();

            hammer.Position = availablePoints[index].Position;

            AddChild(hammer);

            availablePoints.RemoveAt(index);
        }
    }
}