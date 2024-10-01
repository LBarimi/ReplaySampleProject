using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class UpdateManager : MonoBehaviour
{
    public static UpdateManager Instance { get; set; }
    
    private List<IUpdater> _updateList = new();

    private WaitForFixedUpdate _waitForFixedUpdate = new();
    
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
        
        for (var i = 0; i < _updateList.Count; i++)
        {
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
                _updateList[i].OnFixedAfterUpdate(deltaTime);
            }
        }
    }
}