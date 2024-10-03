using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ReplayFileManager : MonoBehaviour
{
    private static readonly bool JSON_PRETTY_PRINT = false;

    private static readonly string EXTENSION = ".json";

    private static readonly string FILE_ROOT_PATH = $"{Application.persistentDataPath}";

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
        return Path.Combine(FILE_ROOT_PATH, $"replay_{chunkCount}.json");
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
    
    private static async Task<List<ReplayData>> CacheDataToReplayData(List<ReplayCacheData> cacheDataList)
    {
        var replayDataList = new List<ReplayData>();

        var writeFixedStepSet = new HashSet<ulong>();

        for (var i = 0; i < cacheDataList.Count; i++)
        {
            // 이미 저장된 프레임의 데이터는 건너뛴다.
            if (writeFixedStepSet.Contains(cacheDataList[i].GetCurFixedStep()))
                continue;
            
            var curFixedStep = cacheDataList[i].GetCurFixedStep();
            
            writeFixedStepSet.Add(curFixedStep);
            
            // 동일한 프레임의 데이터를 수집하고.
            var sameFrameCacheDataList = cacheDataList.FindAll(x => x.GetCurFixedStep() == curFixedStep);

            // 저장용 데이터로 컨버팅 한다.
            var data = new ReplayData();
            data.SetCurFixedStep(curFixedStep);

            // 동일한 프레임의 데이터를 스트링 하나에 압축한다.
            var stringData = string.Empty;
            
            foreach (var cache in sameFrameCacheDataList)
            {
                var d = cache.ConvertToStringData();
                stringData += string.IsNullOrEmpty(stringData) ? d : $"/{d}";
            }
            
            data.SetData(stringData);
            
            replayDataList.Add(data);
        }
        
        return replayDataList;
    }

    public static async Task SaveDataAsync(List<ReplayCacheData> cacheDataList, int chunkCount)
    {
        if (cacheDataList.IsNullOrEmpty())
            return;

        var replayData = await CacheDataToReplayData(cacheDataList);

        var filePath = GetChunkFileName(chunkCount);

        var jsonString = JsonUtility.ToJson(new Wrapper<List<ReplayData>>(replayData), JSON_PRETTY_PRINT);

        await using var writer = new StreamWriter(filePath, false, new System.Text.UTF8Encoding(false));
    
        await writer.WriteAsync(jsonString);
    }
    
    public static List<ReplayData> LoadData(int chunkCount)
    {
        var filePath = GetChunkFileName(chunkCount);

        if (File.Exists(filePath) == false)
            return null;
        
        using var reader = new StreamReader(filePath);
            
        var jsonString = reader.ReadToEnd();

        var wrapper = JsonUtility.FromJson<Wrapper<List<ReplayData>>>(jsonString);

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