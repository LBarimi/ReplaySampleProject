using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class ReplayBinaryDataManager : MonoBehaviour
{
    private static readonly bool JSON_PRETTY_PRINT = false;

    private static readonly string EXTENSION = ".json";

    public static void Clear()
    {
        if (Directory.Exists(Application.persistentDataPath) == false) 
            return;
        
        var files = Directory.GetFiles(Application.persistentDataPath);

        foreach (var fullPath in files)
        {
            File.Delete(fullPath);
        }
    }
    
    private static string GetChunkFileName(int chunkCount = 0)
    {
        return Path.Combine(Application.persistentDataPath, $"replay_{chunkCount}.json");
    }

    public static int GetTotalChunkCount()
    {
        if (Directory.Exists(Application.persistentDataPath) == false)
            return 0;

        var files = Directory.GetFiles(Application.persistentDataPath);

        if (files.IsNullOrEmpty())
            return 0;
        
        var maxIndex = 0;

        foreach (var fullPath in files)
        {
            var file = Path.GetFileName(fullPath);

            if (file.Contains(EXTENSION) == false)
                continue;

            var parts = file
                .Replace(EXTENSION, string.Empty)
                .Split('_');

            if (parts.Length > 1 && int.TryParse(parts[1], out var index))
            {
                if (index > maxIndex)
                {
                    maxIndex = index;
                }
            }
        }

        return maxIndex;
    }

    public static async Task SaveDataAsync(List<ReplayRecordData> dataList, int chunkCount)
    {
        var filePath = GetChunkFileName(chunkCount);

        var jsonString = JsonUtility.ToJson(new Wrapper<List<ReplayRecordData>>(dataList), JSON_PRETTY_PRINT);

        await using var writer = new StreamWriter(filePath);
        
        await writer.WriteAsync(jsonString);
    }
    
    public static List<ReplayRecordData> LoadData(int chunkCount)
    {
        var filePath = GetChunkFileName(chunkCount);

        if (File.Exists(filePath) == false)
            return null;
        
        using var reader = new StreamReader(filePath);
            
        var jsonString = reader.ReadToEnd();

        var wrapper = JsonUtility.FromJson<Wrapper<List<ReplayRecordData>>>(jsonString);

        return wrapper.data;
    }
}

[System.Serializable]
public class Wrapper<T>
{
    public T data;
    
    public Wrapper(T data)
    {
        this.data = data;
    }
}