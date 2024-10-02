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

    // 현재 리플레이 재생 중인지.
    public static bool IsReplaying() => Instance != null && Instance._isReplaying;

    public static void SetReplaying(bool isReplaying)
    {
        if (Instance == null)
            return;

        Instance._isReplaying = isReplaying;
    }
    
    private bool _isReplaying;
    
    // 현재 녹화 상태인지.
    public static bool IsRecording() => Instance != null && Instance._isRecording;

    public static void SetRecording(bool isRecording)
    {
        if (Instance == null)
            return;

        Instance._isRecording = isRecording;
    }
    
    private bool _isRecording;

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
        // (임시) 라운드 시작. 
        if (_curFixedStep == 0)
        {
            //RoundManager.Instance.StartReplay();
            RoundManager.Instance.StartRound();
        }
        
        _curFixedStep++;
    }
}
