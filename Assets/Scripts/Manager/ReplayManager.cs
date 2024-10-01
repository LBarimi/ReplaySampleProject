using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class ReplayManager : MonoBehaviour
{
    private static ReplayManager Instance { get; set; }

    public static ulong GetCurFixedStep() => Instance == null ? 0 : Instance._curFixedStep;

    public static float GetDeltaTime() => Time.deltaTime;
    
    public static float GetFixedDeltaTime() => Time.fixedDeltaTime;
    
    private ulong _curFixedStep;

    public static bool IsReplaying() => Instance != null && Instance._isReplaying;

    public static void SetReplaying(bool isReplaying)
    {
        if (Instance == null)
            return;

        Instance._isReplaying = isReplaying;
    }
    
    private bool _isReplaying;
    
    private void Awake()
    {
        Instance = this;

        Application.targetFrameRate = 60;
        
        _curFixedStep = 0;
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        _curFixedStep++;
    }
}
