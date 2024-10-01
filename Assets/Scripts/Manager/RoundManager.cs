using UnityEngine;

public sealed class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }
    
    public void StartRound()
    {
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
}