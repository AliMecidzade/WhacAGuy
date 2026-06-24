using System.Collections.Generic;
using System.Linq;
using WhacAGuy.Gameplay.Score;

namespace WhacAGuy.Core;

public class HighScoreList
{
    private static int MaxCount = 10; 
    private List<int> _highScores;

    public  HighScoreList(List<int> highScores)
    {
        
        _highScores = highScores ?? new List<int>();
    }

    
    public void Add(int score) 
    {
        _highScores.Add(score);
        _highScores = _highScores.OrderByDescending(s => s).Take(MaxCount).ToList(); 
    }

    public IReadOnlyList<int> Scores => _highScores;

}