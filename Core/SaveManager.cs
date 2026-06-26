using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class SaveManager : Node
{
    public static SaveManager Instance;

    public override void _Ready()
    {
        Instance = this;
    }

    public void Save(SaveData data)
    {
        var scoresArr = new Array();
        foreach (int s in data.Scores ?? new())
            scoresArr.Add(s);

        var dict = new Dictionary
        {
            ["HighScore"] = data.HighScore,
            ["Scores"] = scoresArr
        };
        string json = Json.Stringify(dict);
        using var file = FileAccess.Open("user://save.json", FileAccess.ModeFlags.Write);
        file.StoreString(json);
    }

    public SaveData Load()
    {
        if (!FileAccess.FileExists("user://save.json"))
            return new SaveData();

        using var file = FileAccess.Open("user://save.json", FileAccess.ModeFlags.Read);
        string json = file.GetAsText();

        if (string.IsNullOrWhiteSpace(json))
            return new SaveData();

        var jsonObj = new Json();
        Error error = jsonObj.Parse(json);
        if (error != Error.Ok)
            return new SaveData();

        var dict = jsonObj.Data.AsGodotDictionary();
        int highScore = 0;
        if (dict.ContainsKey("HighScore"))
            highScore = (int)(long)dict["HighScore"];

        var scores = new List<int>();
        if (dict.ContainsKey("Scores"))
        {
            var arr = dict["Scores"].AsGodotArray();
            foreach (var v in arr)
                scores.Add((int)(long)v);
        }

        return new SaveData
        {
            HighScore = highScore,
            Scores = scores
        };
    }
}