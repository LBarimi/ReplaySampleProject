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

    private static bool _isPendingStartRound = false;

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
        _curFixedStep++;

        if (_isPendingStartRound)
        {
            _isPendingStartRound = false;
            StartRoundInternal();
        }
    }

    public static void StartRecord()
    {
        if (IsRecording())
            return;

        _isPendingStartRound = true;
    }
    
    private static void StartRoundInternal()
    {
        if (IsRecording())
            return;

        if (IsReplaying())
            ReplayPlayer.Stop();
        
        // 녹화 시작.
        ReplayRecorder.Play();
        
        // 플레이어 유닛 스폰.
        var playerGo = Utils.SpawnGameObject("PlayerUnit");
        
        PlayerUnit playerUnit = null;
        
        if (playerGo.TryGetComponent(typeof(PlayerUnit), out var com))
            playerUnit = com as PlayerUnit;

        if (playerUnit == null)
            return;

        // 임시 생성 위치.
        playerUnit.transform.position = new Vector3(0, 1.5f, 1.4f);
        playerUnit.Init();
    }
    
    public static void StartReplay()
    {
        if (IsReplaying())
            return;

        if (IsRecording())
            ReplayRecorder.Stop();
        
        // 필드에 존재하는 모든 유닛 제거.
        foreach (var unit in UnitManager.GetAllUnitList())
            unit.MarkForRemoval();
        
        // 분할 저장 개수를 가져온다.
        var totalChunkCount = ReplayFileManager.GetTotalChunkCount() + 1;

        // 분할된 파일을 하나의 데이터로 합치는 작업을 진행한다.
        var dataList = new List<ReplayData>();
        
        for (var i = 0; i < totalChunkCount; i++)
        {
            // 분할된 파일을 로드하고.
            var replayDataList = ReplayFileManager.LoadData(i);

            // 리스트에 추가한다.
            foreach (var item in replayDataList)
            {
                dataList.Add(item);
            }
        }
        
        UniqueIdGenerator.Clear();
        
        ReplayPlayer.ReplayDataToCacheData(dataList);
        ReplayPlayer.Play();
    }
}
