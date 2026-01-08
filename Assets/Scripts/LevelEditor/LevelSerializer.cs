using System.IO;
using UnityEngine;

public static class LevelSerializer
{
    public static void SaveLevel(LevelData data, string filePath)
    {
        var json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
    }

    public static LevelData LoadLevel(string filePath)
    {
        if (!File.Exists(filePath)) return null;
        var json = File.ReadAllText(filePath);
        return JsonUtility.FromJson<LevelData>(json);
    }
}