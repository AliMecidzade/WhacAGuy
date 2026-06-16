using Godot;

namespace WhacAGuy.Gameplay.GameProgression;

public class RoundProgression
{
    private int _currentRound = 1;

    public int CurrentRound => _currentRound;

    public void AdvanceRound()
    {
        _currentRound++;
    }

    public RoundData GetRoundData()
    {
        return new RoundData
        {
            HammerCount =
                Mathf.Min(5, 4 + (_currentRound - 1) / 5),

            SpawnDelay =
                Mathf.Max(
                    1.0f,
                    2.5f - (_currentRound - 1) * 0.05f),

            AttackDelay =
                Mathf.Clamp(
                    1f - (_currentRound - 1) * 0.015f,
                    0.3f,
                    1f )
        };
    }
}