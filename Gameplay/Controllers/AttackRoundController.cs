using Godot;

namespace WhacAGuy.Gameplay.Controllers;

public partial class AttackRoundController : IEventBusInjectable
{
    private EventBus _eventBus;

    private bool _playerHit;
    private bool _running;

    private int _finishedHammers;
    private int _expectedHammers;

    private float _bestQuality;

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

        _bestQuality = 0f;
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

        float quality = 0f;

        if (attackDelay > 0f)
            quality = timeInside / attackDelay;

        quality = Mathf.Clamp(quality, 0f, 1f);

        if (quality > _bestQuality)
            _bestQuality = quality;

        if (_finishedHammers < _expectedHammers)
            return;

        _running = false;

        GD.Print($"END ROUND quality={_bestQuality} hit={_playerHit}");

        _eventBus.EmitAttackRoundFinished(
            _bestQuality,
            _playerHit);
    }
}