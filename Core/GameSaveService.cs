using System;
using System.Linq;

namespace WhacAGuy.Core;

public class GameSaveService
{
    public void SaveOnDeath(int score)
    {
        if (SaveManager.Instance == null)
            return;

        SaveData data = SaveManager.Instance.Load();
        var scores = new HighScoreList(data.Scores);
        scores.Add(score);
        int highScore = Math.Max(data.HighScore, score);

        SaveManager.Instance.Save(new SaveData
        {
            HighScore = highScore,
            Scores = scores.Scores.ToList()
        });
    }
}