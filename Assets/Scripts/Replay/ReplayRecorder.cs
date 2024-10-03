using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

public enum REPLAY_ACTION_TYPE
{
    NONE,
    SPAWN_UNIT,
    DESPAWN_UNIT,
    INPUT_VECTOR,
    SYNC_TRANSFORM,
    SYNC_VELOCITY,
}

/// <summary>
/// 리플레이 녹화를 담당하는 클래스.
/// </summary>
public sealed class ReplayRecorder
{
    private static List<ReplayCacheData> _cacheDataList = new();

    // 데이터 분할 기준 : default 3kb.
    private static readonly float CHUNK_SIZE = 3;

    // 데이터가 현재까지 분할된 횟수.
    private static int _curChunkCount;

    private static ulong _startRecordFixedStep;

    private static ulong GetStartRecordFixedStep()
    {
        return ReplayManager.GetCurFixedStep() - _startRecordFixedStep;
    }
    
    // 녹화 시작.
    public static void Play()
    {
        if (ReplayManager.IsReplaying())
            return;
        
        _cacheDataList.Clear();
        _curChunkCount = 0;
        _startRecordFixedStep = ReplayManager.GetCurFixedStep();
        
        ReplayManager.SetRecording(true);
        
        ReplayFileManager.Clear();
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

    // spawn, despawn. (string visualKey|position)
    public static void Set(REPLAY_ACTION_TYPE replayActionType, int id, string visualKey, float x, float y, float z)
    {
        if (ReplayManager.IsRecording() == false)
            return;
        
        var data = new ReplayCacheData();
        data.SetCurFixedStep(GetStartRecordFixedStep());
        data.SetId(id);
        data.SetType(replayActionType);
        data.SetVisualKey(visualKey);
        data.SetPosition(x.ToPercentage(), y.ToPercentage(), z.ToPercentage());
        
        Process(data);
    }
    
    // input. (vector)
    public static void Set(REPLAY_ACTION_TYPE replayActionType, int id, int x, int y)
    {
        if (ReplayManager.IsRecording() == false)
            return;
        
        var data = new ReplayCacheData();
        data.SetCurFixedStep(GetStartRecordFixedStep());
        data.SetId(id);
        data.SetType(replayActionType);
        data.SetPosition(x, y, 0);
        
        Process(data);
    }
    
    // sync transform. (position|quaternion)
    public static void Set(REPLAY_ACTION_TYPE replayActionType, int id, float x, float y, float z, float qx, float qy, float qz, float qw)
    {
        if (ReplayManager.IsRecording() == false)
            return;
        
        var data = new ReplayCacheData();
        data.SetCurFixedStep(GetStartRecordFixedStep());
        data.SetId(id);
        data.SetType(replayActionType);
        data.SetPosition(x.ToPercentage(), y.ToPercentage(), z.ToPercentage());
        data.SetQuaternion(qx.ToPercentage(), qy.ToPercentage(), qz.ToPercentage(), qw.ToPercentage());

        Process(data);
    }

    public static void Set(REPLAY_ACTION_TYPE replayActionType, int id, float vx, float vy, float vz)
    {
        if (ReplayManager.IsRecording() == false)
            return;
        
        var data = new ReplayCacheData();
        data.SetCurFixedStep(GetStartRecordFixedStep());
        data.SetId(id);
        data.SetType(replayActionType);
        data.SetVelocity(vx.ToPercentage(), vy.ToPercentage(), vz.ToPercentage());

        Process(data);
    }

    private static void Process(ReplayCacheData data)
    {
        // 캐시 데이터 추가.
        _cacheDataList.Add(data);

        // 분할저장의 기준을 초과했는지 검사하고
        if (IsExceedingChunkSize())
        {
            // 쌓인 데이터 내보내기.
            Flush();
        
            // 내보낸 이후 캐시 목록 정리.
            _cacheDataList.Clear();
        }
    }
    
    // 녹화된 데이터 내보내기.
    private static void Flush()
    {
        var cacheDataList = _cacheDataList.ToList();
        var curChunkCount = _curChunkCount++;
        
        Task.Run(() =>
        {
            try
            {
                ReplayFileManager.SaveDataAsync(cacheDataList, curChunkCount);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Error] ReplayRecorder::Flush Exception:{ex.Message}");
            }
        });
    }
    
    // 단위 : KB.
    private static float GetSize()
    {
        var totalMemory = _cacheDataList.Sum(data => data.GetSizeInBytes());
        return totalMemory.ConvertBytesToKB().Truncate();
    }

    // 분할저장의 기준을 초과했는지 검사.
    private static bool IsExceedingChunkSize()
    {
        return GetSize() >= CHUNK_SIZE;
    }
}

#region WriteData

[Serializable]
public sealed class ReplayData
{
    [SerializeField]
    private ulong f; // curFixedStep.
    
    [SerializeField]
    private string d; // data.
    
    public ulong GetCurFixedStep() 
        => f;

    public void SetCurFixedStep(ulong value) 
        => f = value;

    public string GetData() 
        => d;

    public void SetData(string value)
        => d = value;
}

#endregion

#region CacheData

[Serializable]
public sealed class ReplayCacheData
{
    private ulong _curFixedStep;

    private REPLAY_ACTION_TYPE _actionType;

    private int _id;

    private string _visualKey;

    private int _x, _y, _z;
    private int _qx, _qy, _qz, _qw;
    private int _vx, _vy, _vz;

    private bool _isInput;

    public void SetId(int id)
        => _id = id;

    public int GetId()
        => _id;

    public void SetType(REPLAY_ACTION_TYPE type)
        => _actionType = type;

    public REPLAY_ACTION_TYPE GetType()
        => _actionType;

    public void SetCurFixedStep(ulong curFixedStep)
        => _curFixedStep = curFixedStep;

    public ulong GetCurFixedStep()
        => _curFixedStep;

    public void SetVisualKey(string visualKey)
        => _visualKey = visualKey;

    public string GetVisualKey()
        => _visualKey;

    public void SetPosition(int x, int y, int z)
    {
        _x = x;
        _y = y;
        _z = z;
    }

    public (int x, int y, int z) GetPosition()
        => (_x, _y, _z);

    public void SetQuaternion(int qx, int qy, int qz, int qw)
    {
        _qx = qx;
        _qy = qy;
        _qz = qz;
        _qw = qw;
    }

    public (int qx, int qy, int qz, int qw) GetQuaternion()
        => (_qx, _qy, _qz, _qw);

    public void SetVelocity(int vx, int vy, int vz)
    {
        _vx = vx;
        _vy = vy;
        _vz = vz;
    }

    public (int vx, int vy, int vz) GetVelocity()
        => (_vx, _vy, _vz);

    public void SetInput(bool isInput)
    {
        _isInput = isInput;
    }

    public bool GetInput()
        => _isInput;

    private int GetStringSize(string s)
    {
        return string.IsNullOrEmpty(s) ? 0 : System.Text.Encoding.UTF8.GetByteCount(s);
    }

    public int GetSizeInBytes()
    {
        var size = 0;

        size += sizeof(ulong);
        size += sizeof(int) * 13;
        size += sizeof(bool);

        if (string.IsNullOrEmpty(_visualKey) == false)
        {
            size += sizeof(char) * _visualKey.Length;
        }

        size += IntPtr.Size;

        return size;
    }

    public static ReplayCacheData ConvertToCacheData(string stringData)
    {
        const string DELIMITER = "|";

        var cacheData = new ReplayCacheData();
        
        var split = stringData.Split(DELIMITER);

        if (split.IsNullOrEmpty())
            return null;

        var index = 0;

        if (int.TryParse(split[index++], out var data))
            cacheData._actionType = (REPLAY_ACTION_TYPE)data;

        if (int.TryParse(split[index++], out var id))
            cacheData._id = id;

        cacheData._visualKey = split[index++];

        if (int.TryParse(split[index++], out var x))
            cacheData._x = x;
        if (int.TryParse(split[index++], out var y))
            cacheData._y = y;
        if (int.TryParse(split[index++], out var z))
            cacheData._z = z;
        
        if (int.TryParse(split[index++], out var qx))
            cacheData._qx = qx;
        if (int.TryParse(split[index++], out var qy))
            cacheData._qy = qy;
        if (int.TryParse(split[index++], out var qz))
            cacheData._qz = qz;
        if (int.TryParse(split[index++], out var qw))
            cacheData._qw = qw;
        
        if (int.TryParse(split[index++], out var vx))
            cacheData._vx = vx;
        if (int.TryParse(split[index++], out var vy))
            cacheData._vy = vy;
        if (int.TryParse(split[index++], out var vz))
            cacheData._vz = vz;

        if (int.TryParse(split[index++], out var input))
            cacheData._isInput = input == 1;
        
        return cacheData;
    }
    
    public string ConvertToStringData()
    {
        const string DELIMITER = "|";
        
        var sb = new StringBuilder();

        if (_actionType != REPLAY_ACTION_TYPE.NONE)
            sb.Append($"{(int)_actionType}");
        sb.Append(DELIMITER);

        if (_id != 0)
            sb.Append($"{_id}");
        sb.Append(DELIMITER);

        if (string.IsNullOrEmpty(_visualKey) == false)
            sb.Append($"{_visualKey}");
        sb.Append(DELIMITER);

        // position.
        if (_x != 0)
            sb.Append($"{_x}");
        sb.Append(DELIMITER);
        
        if (_y != 0)
            sb.Append($"{_y}");
        sb.Append(DELIMITER);
        
        if (_z != 0)
            sb.Append($"{_z}");
        sb.Append(DELIMITER);

        // quaternion
        if (_qx != 0)
            sb.Append($"{_qx}");
        sb.Append(DELIMITER);
        
        if (_qy != 0)
            sb.Append($"{_qy}");
        sb.Append(DELIMITER);
        
        if (_qz != 0)
            sb.Append($"{_qz}");
        sb.Append(DELIMITER);
        
        if (_qw != 0)
            sb.Append($"{_qw}");
        sb.Append(DELIMITER);
        
        // velocity.
        if (_vx != 0)
            sb.Append($"{_vx}");
        sb.Append(DELIMITER);
        
        if (_vy != 0)
            sb.Append($"{_vy}");
        sb.Append(DELIMITER);
        
        if (_vz != 0)
            sb.Append($"{_vz}");
        sb.Append(DELIMITER);
        
        // input.
        if (_isInput)
            sb.Append($"{(_isInput ? 1 : 0)}");
        sb.Append(DELIMITER);
        
        return sb.ToString();
    }
}

#endregion