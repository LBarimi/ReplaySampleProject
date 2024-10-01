using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class ReplayManager : MonoBehaviour
{
    private static ReplayManager Instance { get; set; }

    public static ulong GetCurFixedStep() => Instance == null ? 0 : Instance._curFixedStep;
    
    public static float GetFixedDeltaTime() => Time.fixedDeltaTime;
    
    private ulong _curFixedStep;

    private void Awake()
    {
        Instance = this;

        //QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
        
        _curFixedStep = 0;
    }

    private void FixedUpdate()
    {
        _curFixedStep++;
    }
}
