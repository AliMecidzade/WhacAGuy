using Godot;

namespace WhacAGuy.Gameplay.Controllers;

public partial class AttackRoundController : IEventBusInjectable
{
    private EventBus _eventBus;

    private bool _playerHit;
    private bool _running;

    private int _finishedHammers;
    private int _expectedHammers;

    private float _lastExitTimeInside;
    private float _lastExitAttackDelay;
    private bool _hasLastExit;

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

        _lastExitTimeInside = 0f;
        _lastExitAttackDelay = 0f;
        _hasLastExit = false;
    }

    public void NotifyHammerExited(
        float timeInside,
        float attackDelay)
    {
        _lastExitTimeInside = timeInside;
        _lastExitAttackDelay = attackDelay;
        _hasLastExit = true;
    }

    public void RegisterHit()
    {
        if (!_running)
            return;

        _playerHit = true;
    }

    public void NotifyHammerFinished(
        float timeInside,
        float attackDelay)
    {
        if (!_running)
            return;

        _finishedHammers++;

        if (_finishedHammers < _expectedHammers)
            return;

        _running = false;

        float usedTimeInside = _hasLastExit ? _lastExitTimeInside : timeInside;
        float usedAttackDelay = _hasLastExit ? _lastExitAttackDelay : attackDelay;

        float quality = 0f;
        if (usedAttackDelay > 0f)
            quality = usedTimeInside / usedAttackDelay;
        quality = Mathf.Clamp(quality, 0f, 1f);

        GD.Print($"END ROUND quality={quality} hit={_playerHit}");

        _eventBus.EmitAttackRoundFinished(
            quality,
            _playerHit);
    }
}