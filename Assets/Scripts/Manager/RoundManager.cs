using UnityEngine;

public sealed class RoundManager : MonoBehaviour
{
    private void Awake()
    {
    }

    private void Start()
    {
        // TestCase
        StartRound();
    }
    
    public void StartRound()
    {
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