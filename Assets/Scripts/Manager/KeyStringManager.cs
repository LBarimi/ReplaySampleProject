using System.Collections.Generic;
using UnityEngine;

public static class KeyStringManager
{
    private static Dictionary<string, string> DataDict { get; set; } = new()
    {
        // PrefabPath.
        { "PlayerUnit", "Prefabs/PlayerUnit" },
        
        // VisualPath.
        { "Visual_UnityChan", "Visual/Visual_UnityChan" }
    };

    public static string GetString(string key)
    {
        if (DataDict.ContainsKey(key) == false)
        {
            Debug.LogError($"[ERROR] KeyStringManager::GetString {key} is notfound.");
            return string.Empty;
        }

        return DataDict[key];
    }
}