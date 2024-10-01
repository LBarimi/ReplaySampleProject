using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private static List<ReplayRecordData> _cache = new();
    
    // 녹화 시작.
    public static void Play()
    {
        if (ReplayManager.IsReplaying())
            return;
        
        _cache.Clear();
        
        ReplayManager.SetRecording(true);
    }

    // 녹화 종료.
    public static void Stop()
    {
        if (ReplayManager.IsRecording() == false)
            return;
        
        _cache.Clear();
        
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
        
        _cache.Add(data);
    }
    
    // input. (boolean)
    public static void Set(REPLAY_ACTION_TYPE replayActionType, bool isInput)
    {
        if (ReplayManager.IsRecording() == false)
            return;
        
        var data = new ReplayRecordData();
        data.SetCurFixedStep(ReplayManager.GetCurFixedStep());
        data.SetInput(isInput);
        
        _cache.Add(data);
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

        _cache.Add(data);
    }

    public static void Set(REPLAY_ACTION_TYPE replayActionType, float vx, float vy, float vz)
    {
        if (ReplayManager.IsRecording() == false)
            return;
        
        var data = new ReplayRecordData();
        data.SetCurFixedStep(ReplayManager.GetCurFixedStep());
        data.SetType(replayActionType);
        data.SetVelocity(vx.ToPercentage(), vy.ToPercentage(), vz.ToPercentage());
        
        _cache.Add(data);
    }

    public static void ShowCurCacheSize()
    {
        var totalMemory = _cache.Sum(data => data.GetSizeInBytes());
        var kb = totalMemory.ConvertBytesToKB().Truncate();
        
        Debug.Log($"[Cur CacheSize][{totalMemory} byte][{kb} kb]");
    }
    
    // 녹화된 데이터 내보내기.
    public static void Flush()
    {
        if (_cache.IsNullOrEmpty())
            return;
    }
}
