using WhacAGuy.Scenes.DodgeFeedback;

public class PlayerScore
{
    private int _score = 0;

    public int Score => _score;

    public void AddScore(float timeInside, bool playerHit, float maxTime)
    {
        _score += CalculateRoundScore(timeInside, playerHit, maxTime);
    }

    public int CalculateRoundScore(float timeInside, bool playerHit, float maxTime)
    {
        if (playerHit)
            return 0;

        float timeLeft = maxTime - timeInside;

        // More points the closer to the wire you cut it.
        if (timeLeft <= 0.1f)
            return 100;   // nearly got hit

        if (timeLeft <= 0.3f)
            return 50;    // close call

        if (timeLeft <= 0.5f)
            return 10;    // decent dodge

        return 0;         // escaped early, no bonus
    }

    public DodgeRating CalculateRating(float timeInside, bool playerHit, float maxTime)
    {
        if (playerHit)
            return DodgeRating.NotBad;

        float timeLeft = maxTime - timeInside;

        if (timeLeft <= 0.1f)
            return DodgeRating.Excellent;

        if (timeLeft <= 0.3f)
            return DodgeRating.Good;

        return DodgeRating.NotBad;
    }
}