using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class UpdateManager : MonoBehaviour
{
    private List<IUpdater> _updateList = new();

    private void Awake()
    {
        _updateList.Clear();
    }
    
    private void FixedUpdate()
    {
        var deltaTime = ReplayManager.GetFixedDeltaTime();
        
        for (var i = 0; i < _updateList.Count; i++)
        {
            _updateList[i].OnFixedUpdate(deltaTime);
        }
    }
}