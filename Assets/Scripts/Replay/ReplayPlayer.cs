using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 리플레이 재생을 담당하는 클래스.
/// </summary>
public sealed class ReplayPlayer : MonoBehaviour
{
    private static List<ReplayCacheData> _cacheDataList = new();
    
    private static ulong _curFixedStep;

    public static void Play()
    {
        if (ReplayManager.IsRecording())
            return;

        _curFixedStep = 0;
        
        ReplayManager.SetReplaying(true);
        
        UpdateManager.Instance.SetFixedUpdateBefore(OnFixedUpdate);
    }

    public static void Stop()
    {
        if (ReplayManager.IsReplaying() == false)
            return;
        
        ReplayManager.SetReplaying(false);
        
        UpdateManager.Instance.RemoveFixedUpdateBefore(OnFixedUpdate);
    }

    public static void ReplayDataToCacheData(List<ReplayData> dataList)
    {
        _cacheDataList.Clear();
        
        foreach (var item in dataList)
        {
            var stringData = item.GetData();
            if (string.IsNullOrEmpty(stringData))
                continue;
            
            var split = stringData.Split('/');
            if (split.IsNullOrEmpty())
                continue;

            foreach (var src in split)
            {
                var cacheData = ReplayCacheData.ConvertToCacheData(src);

                if (cacheData == null)
                    continue;
                
                cacheData.SetCurFixedStep(item.GetCurFixedStep());
                
                _cacheDataList.Add(cacheData);
            }
        }
    }
    
    private static void OnFixedUpdate(float deltaTime)
    {
        if (ReplayManager.IsReplaying() == false)
            return;

        if (_cacheDataList.IsNullOrEmpty())
            return;

        var isEnd = false;
        
        foreach (var item in _cacheDataList)
        {
            if (item.GetCurFixedStep() == _curFixedStep)
            {
                ProcessEvent(item);

                isEnd = _cacheDataList.IndexOf(item) == _cacheDataList.Count - 1;
            }
        }

        if (isEnd)
        {
            Stop();
            return;
        }

        _curFixedStep++;
    }

    private static void ProcessEvent(ReplayCacheData data)
    {
        switch (data.GetType())
        {
            case REPLAY_ACTION_TYPE.NONE:
                break;
            case REPLAY_ACTION_TYPE.SPAWN_UNIT:
            {
                // 플레이어 유닛 스폰.
                var playerGo = Utils.SpawnGameObject("PlayerUnit");
        
                PlayerUnit playerUnit = null;
        
                if (playerGo.TryGetComponent(typeof(PlayerUnit), out var com))
                    playerUnit = com as PlayerUnit;

                if (playerUnit == null)
                    return;

                var x = data.GetPosition().x.ToRate();
                var y = data.GetPosition().y.ToRate();
                var z = data.GetPosition().z.ToRate();

                playerUnit.transform.position = new Vector3(x, y, z);
                playerUnit.SetId(data.GetId());
                playerUnit.Init();
                
                break;
            }
            case REPLAY_ACTION_TYPE.DESPAWN_UNIT:
                break;
            case REPLAY_ACTION_TYPE.INPUT_VECTOR:
            {
                var x = data.GetPosition().x;
                var y = data.GetPosition().y;
                
                var target = UnitManager.Get(data.GetId());

                target.GetInputController().movementInputValue = new Vector2Int(x, y);
                
                break;
            }
            case REPLAY_ACTION_TYPE.SYNC_TRANSFORM:
            {
                var x = data.GetPosition().x.ToRate();
                var y = data.GetPosition().y.ToRate();
                var z = data.GetPosition().z.ToRate();
                
                var qx = data.GetQuaternion().qx.ToRate();
                var qy = data.GetQuaternion().qy.ToRate();
                var qz = data.GetQuaternion().qz.ToRate();
                var qw = data.GetQuaternion().qw.ToRate();

                var target = UnitManager.Get(data.GetId());
                
                target.transform.position = new Vector3(x, y, z);
                target.transform.rotation = new Quaternion(qx, qy, qz, qw);
                
                target.transform.SetTruncatePosition();
                target.transform.SetTruncateRotation();
                
                break;
            }
            case REPLAY_ACTION_TYPE.SYNC_VELOCITY:
            {
                var target = UnitManager.Get(data.GetId());

                var vx = data.GetVelocity().vx.ToRate();
                var vy = data.GetVelocity().vy.ToRate();
                var vz = data.GetVelocity().vz.ToRate();
                
                target.GetRigidBody().velocity = new Vector3(vx, vy, vz);
                
                break;
            }
        }
    }
}
