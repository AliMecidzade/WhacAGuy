using Godot;

namespace WhacAGuy.Gameplay.Controllers;

public partial class AttackRoundController : IEventBusInjectable
{
    private EventBus _eventBus;

    private bool _playerHit = false;
    private bool _running = false;
    private int _finishedHammers;
    private int _expectedHammers;
    private float _maxTimeInside;   
    private float _maxTime;         

    public void Init(EventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public void StartRound(int totalHammerCount)
    {
        _playerHit = false;
        _running = true;
        _finishedHammers = 0;
        _expectedHammers = totalHammerCount;
        _maxTimeInside = 0f;
        _maxTime = 0f;
    }

    public void RegisterHit()
    {
        if (!_running)
            return;

        _playerHit = true;
    }

    public void NotifyHammerFinished(float timeInside, float maxTime)
    {
        if (!_running)
            return;

        _finishedHammers++;

        if (timeInside > _maxTimeInside)
            _maxTimeInside = timeInside;

        if (maxTime > _maxTime)
            _maxTime = maxTime;

        if (_finishedHammers < _expectedHammers)
            return;

        _running = false;

        _eventBus.EmitAttackRoundFinished(_maxTimeInside, _playerHit, _maxTime);
    }
}