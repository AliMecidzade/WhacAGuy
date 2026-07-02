using Godot;
using System;

public partial class TimerBar : Control , IEventBusInjectable
{
	[Export] private ProgressBar _timerBar;
	private EventBus _eventBus; 
	private Tween _tween;
	private bool _isVisible; 
	public override void _Ready()
	{

		
		_isVisible = false; 
		_timerBar.Visible = false;
	}

	public void Start()
	{
		_eventBus.Connect(EventBus.SignalName.RoundAttackPhaseStarted,
			new Callable(this, nameof(ShowTimerBar)));
		_eventBus.Connect(EventBus.SignalName.AttackRoundFinished,
			new Callable(this, nameof(HideTimerBar)));
		_eventBus.Connect(EventBus.SignalName.GamePaused,
			new Callable(this, nameof(OnGamePaused)));
		_eventBus.Connect(EventBus.SignalName.GameUnpaused,
			new Callable(this, nameof(OnGameUnpaused)));
	}

	private void OnGamePaused()
	{
		if (_tween != null)
			_tween.SetSpeedScale(0);
	}

	private void OnGameUnpaused()
	{
		if (_tween != null)
			_tween.SetSpeedScale(1);
	}
	public void Init(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void ShowTimerBar(float attackDelay)
	{
		if (!_isVisible)
		{
			_timerBar.Visible = true;
		}
		_timerBar.Value = _timerBar.MaxValue;

		var styleBox = new StyleBoxFlat();
		_timerBar.AddThemeStyleboxOverride("fill", styleBox);

		_tween = CreateTween();
		_tween.SetParallel(true); 

		
		_tween.TweenProperty(_timerBar, "value", _timerBar.MinValue, attackDelay);

		_tween.TweenMethod(
			Callable.From((float t) =>
			{
				styleBox.BgColor = new Color(t, 1f - t, 0f);
			}),
			0f, 1f, attackDelay
		);
	}

	public void HideTimerBar(float dodgeQuality, bool playerHit)
	{
		_isVisible = false;
		_timerBar.Visible = false;
	}
}
