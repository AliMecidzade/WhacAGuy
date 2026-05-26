using Godot;

namespace WhacAGuy.Gameplay.Controllers;

public partial class AttackRoundController : Node, IEventBusInjectable
{
    private EventBus _eventBus;

    private bool _playerHit = false;
    private bool _running = false;

    public void Init(EventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public void StartRound()
    {
        _playerHit = false;
        _running = true;
    }

    public void RegisterHit()
    {
        _playerHit = true;
    }

    public void EndRound(float reactionTime)
    {
        if (!_running)
            return;

        _running = false;

        _eventBus.EmitAttackRoundFinished(reactionTime, _playerHit);
    }
}