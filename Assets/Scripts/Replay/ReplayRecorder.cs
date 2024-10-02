using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum REPLAY_ACTION_TYPE
{
    NONE,
    SPAWN_UNIT,
    DESPAWN_UNIT,
    INPUT_LEFT_DOWN,
    INPUT_LEFT_PRESSED,
    INPUT_LEFT_UP,
    INPUT_RIGHT_DOWN,
    INPUT_RIGHT_PRESSED,
    INPUT_RIGHT_UP,
    INPUT_UP_DOWN,
    INPUT_UP_PRESSED,
    INPUT_UP_UP,
    INPUT_DOWN_DOWN,
    INPUT_DOWN_PRESSED,
    INPUT_DOWN_UP,
    SYNC_TRANSFORM,
    SYNC_VELOCITY,
}

/// <summary>
/// 리플레이 녹화를 담당하는 클래스.
/// </summary>
public class ReplayRecorder
{
    private static List<ReplayRecordData> _cacheDataList = new();

    // 데이터 분할 기준 : default 3kb.
    private static readonly float CHUNK_SIZE = 3;

    // 데이터가 현재까지 분할된 횟수.
    private static int _curChunkCount;
    
    // 녹화 시작.
    public static void Play()
    {
        if (ReplayManager.IsReplaying())
            return;
        
        _cacheDataList.Clear();
        _curChunkCount = 0;
        
        ReplayManager.SetRecording(true);
        
        ReplayBinaryDataManager.Clear();
    }

    // 녹화 종료.
    public static void Stop()
    {
        if (ReplayManager.IsRecording() == false)
            return;
        
        _cacheDataList.Clear();
        _curChunkCount = 0;
        
        ReplayManager.SetRecording(false);
    }

    // spawn, despawn. (string key|position)
    public static void Set(REPLAY_ACTION_TYPE replayActionType, string key, float x, float y, float z)
    {
        if (ReplayManager.IsRecording() == false)
            return;

        var data = new ReplayRecordData();
        data.SetCurFixedStep(ReplayManager.GetCurFixedStep());
        data.SetType(replayActionType);
        data.SetKey(key);
        data.SetPosition(x.ToPercentage(), y.ToPercentage(), z.ToPercentage());
        
        Process(data);
    }
    
    // input. (boolean)
    public static void Set(REPLAY_ACTION_TYPE replayActionType, bool isInput)
    {
        if (ReplayManager.IsRecording() == false)
            return;
        
        var data = new ReplayRecordData();
        data.SetCurFixedStep(ReplayManager.GetCurFixedStep());
        data.SetInput(isInput);
        
        Process(data);
    }
    
    // sync transform. (position|quaternion)
    public static void Set(REPLAY_ACTION_TYPE replayActionType, float x, float y, float z, float qx, float qy, float qz, float qw)
    {
        if (ReplayManager.IsRecording() == false)
            return;
        
        var data = new ReplayRecordData();
        data.SetCurFixedStep(ReplayManager.GetCurFixedStep());
        data.SetType(replayActionType);
        data.SetPosition(x.ToPercentage(), y.ToPercentage(), z.ToPercentage());
        data.SetQuaternion(qx.ToPercentage(), qy.ToPercentage(), qz.ToPercentage(), qw.ToPercentage());

        Process(data);
    }

    public static void Set(REPLAY_ACTION_TYPE replayActionType, float vx, float vy, float vz)
    {
        if (ReplayManager.IsRecording() == false)
            return;
        
        var data = new ReplayRecordData();
        data.SetCurFixedStep(ReplayManager.GetCurFixedStep());
        data.SetType(replayActionType);
        data.SetVelocity(vx.ToPercentage(), vy.ToPercentage(), vz.ToPercentage());

        Process(data);
    }

    private static void Process(ReplayRecordData data)
    {
        // 캐시 데이터 추가.
        _cacheDataList.Add(data);

        // 분할저장의 기준을 초과했는지 검사하고
        if (IsExceedingChunkSize())
        {
            // 쌓인 데이터 내보내기.
            Flush();
        }
    }
    
    // 단위 : KB.
    private static float GetSize()
    {
        var totalMemory = _cacheDataList.Sum(data => data.GetJsonSizeInBytes());
        return totalMemory.ConvertBytesToKB().Truncate();
    }

    // 분할저장의 기준을 초과했는지 검사.
    private static bool IsExceedingChunkSize()
    {
        return GetSize() >= CHUNK_SIZE;
    }
    
    // 녹화된 데이터 내보내기.
    private static void Flush()
    {
        if (_cacheDataList.IsNullOrEmpty())
            return;

        ReplayBinaryDataManager.SaveDataAsync(_cacheDataList.ToList(), _curChunkCount++);
        
        _cacheDataList.Clear();
    }
}
