using Godot;

namespace WhacAGuy.Gameplay.Controllers;

public partial class AttackRoundController : IEventBusInjectable
{
    private EventBus _eventBus;

    private bool _playerHit;
    private bool _running;
    private bool _paused;

    private int _finishedHammers;
    private int _expectedHammers;

    private int _playerOnHammerCount;
    private float _totalTimeOnAnyHammer;
    private float _roundDuration;

    public bool IsRunning => _running;

    public void Pause()
    {
        _paused = true;
    }

    public void Resume()
    {
        _paused = false;
    }

    public void Init(EventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public void StartRound(int totalHammerCount, float attackDelay)
    {
        _playerHit = false;
        _running = true;

        _finishedHammers = 0;
        _expectedHammers = totalHammerCount;

        _playerOnHammerCount = 0;
        _totalTimeOnAnyHammer = 0f;
        _roundDuration = attackDelay;
    }

    public void NotifyHammerEntered()
    {
        if (_paused)
            return;
        _playerOnHammerCount++;
    }

    public void NotifyHammerExited()
    {
        if (_paused)
            return;
        if (_playerOnHammerCount > 0)
            _playerOnHammerCount--;
    }

    public void AccumulateTime(float delta)
    {
        if (!_running || _paused || _playerOnHammerCount == 0)
            return;
        _totalTimeOnAnyHammer += delta;
    }

    public void RegisterHit()
    {
        if (!_running || _paused)
            return;
        _playerHit = true;
    }

    public void NotifyHammerFinished(float timeInside, float attackDelay)
    {
        if (!_running || _paused)
            return;

        _finishedHammers++;

        if (_finishedHammers < _expectedHammers)
            return;

        _running = false;

        float quality = _roundDuration > 0f
            ? Mathf.Clamp(_totalTimeOnAnyHammer / _roundDuration, 0f, 1f)
            : 0f;

        GD.Print($"END ROUND quality={quality} hit={_playerHit}");

        _eventBus.EmitAttackRoundFinished(quality, _playerHit);
    }
}
