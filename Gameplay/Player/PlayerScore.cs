using WhacAGuy.Scenes.DodgeFeedback;

namespace WhacAGuy.Gameplay.Score;

public class PlayerScore
{
    private int _score;

    public int Score => _score;

    public void AddScore(float dodgeQuality)
    {
        _score += CalculateRoundScore(dodgeQuality);
    }

    public int CalculateRoundScore(float dodgeQuality)
    {
        if (dodgeQuality >= 0.9f)
            return 100;

        if (dodgeQuality >= 0.7f)
            return 50;

        if (dodgeQuality >= 0.4f)
            return 10;

        return 1;
    }

    public DodgeRating CalculateRating(float dodgeQuality)
    {
        if (dodgeQuality >= 0.9f)
            return DodgeRating.Excellent;

        if (dodgeQuality >= 0.7f)
            return DodgeRating.Good;

        return DodgeRating.NotBad;
    }
}