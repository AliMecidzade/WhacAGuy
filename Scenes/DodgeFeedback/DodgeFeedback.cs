    using Godot;
using WhacAGuy.Gameplay.Score;

namespace WhacAGuy.Scenes.DodgeFeedback;

public partial class DodgeFeedback : Control
{
    [Export] private Label _ratingMessage;

    public async void Show(DodgeRating dodgeRating)
    {
        switch (dodgeRating)
        {
            case DodgeRating.Excellent:
                _ratingMessage.Text = "Excellent";
                break;
            case DodgeRating.Good:
                _ratingMessage.Text = "Good";
                break;
            case DodgeRating.NotBad:
                _ratingMessage.Text = "Not Bad";
                break;
        }

        Modulate = new Color(1, 1, 1, 1);

        var tween = CreateTween();
        tween.SetParallel();

        tween.TweenProperty(
            this,
            "position",
            Position + new Vector2(0, -40),
            0.5f);

        tween.TweenProperty(
            this,
            "modulate:a",
            0f,
            0.5f);

        await ToSignal(tween, Tween.SignalName.Finished);

        QueueFree();
    }
}