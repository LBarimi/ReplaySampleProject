using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class UpdateManager : MonoBehaviour
{
    public static UpdateManager Instance { get; set; }
    
    private List<IUpdater> _updateList = new();

    private WaitForFixedUpdate _waitForFixedUpdate = new();

    private Action<float> _onFixedUpdate_Before;
    
    public void SetFixedUpdateBefore(Action<float> callback) 
        => _onFixedUpdate_Before += callback;

    public void RemoveFixedUpdateBefore(Action<float> callback) 
        => _onFixedUpdate_Before -= callback;
    
    private void Awake()
    {
        Instance = this;
        
        _updateList.Clear();
    }

    private void Start()
    {
        StartCoroutine(CorFixedAfterUpdate());
    }

    public void Add(IUpdater updater)
    {
        if (_updateList.Contains(updater))
            return;
        
        _updateList.Add(updater);
    }

    public void Remove(IUpdater updater)
    {
        if (_updateList.Contains(updater) == false)
            return;
        
        _updateList.Remove(updater);
    }
    
    public void MarkForRemoval(IUpdater updater)
    {
        if (_updateList.Contains(updater) == false)
            return;

        updater.IsMarkedForRemoval = true;
    }
    
    private void FixedUpdate()
    {
        var deltaTime = ReplayManager.GetFixedDeltaTime();
        
        // 제거 예정인 업데이터를 제거한다.
        for (var i = _updateList.Count - 1; i >= 0; i--)
        {
            if (_updateList[i].IsMarkedForRemoval)
                _updateList.RemoveAt(i);
        }
        
        _onFixedUpdate_Before?.Invoke(deltaTime);
        
        for (var i = 0; i < _updateList.Count; i++)
        {
            if (_updateList[i].IsMarkedForRemoval)
                continue;
            
            _updateList[i].OnFixedUpdate(deltaTime);
        }
    }

    private IEnumerator CorFixedAfterUpdate()
    {
        var deltaTime = ReplayManager.GetFixedDeltaTime();

        while (true)
        {
            yield return _waitForFixedUpdate;
            
            for (var i = 0; i < _updateList.Count; i++)
            {
                if (_updateList[i].IsMarkedForRemoval)
                    continue;
                
                _updateList[i].OnFixedAfterUpdate(deltaTime);
            }
        }
    }
}