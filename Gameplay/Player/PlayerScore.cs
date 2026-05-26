using Godot;

public class PlayerScore
{
    private int _score = 0;

    public int Score
    {
        get { return _score; }
    }

    public void AddScore(float reactionTime, bool playerHit)
    {
        _score += CalculateScore(reactionTime, playerHit);
    }

    public int CalculateScore(float reactionTime, bool playerHit)
    {
        if (playerHit)
            return 0;

        float maxTime = 1.0f;

        // 0.0 sec = perfect
        // 1.0 sec = terrible
        float normalized = Mathf.Clamp(reactionTime / maxTime, 0f, 1f);

        // invert it
        float quality = 1f - normalized;

        // optional curve for better feeling
        quality *= quality;

        return Mathf.RoundToInt(quality * 100);
    }
}