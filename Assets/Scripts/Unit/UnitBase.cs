using UnityEngine;

public class UnitBase : MonoBehaviour, IUpdater, IObject
{
    public bool IsMarkedForRemoval { get; set; }

    // 오브젝트 식별자.
    public ulong id { get; set; }

    public ulong GetId() 
        => id;

    public string _visualKey;

    public void SetVisualKey(string visualKey)
        => _visualKey = visualKey;

    public string GetVisualKey()
        => _visualKey;

    // 스탯 관리.
    private StatController _statController;

    protected void SetStatController(StatController statController)
        => _statController = statController;

    protected StatController GetStatController()
        => _statController;

    // 입력 관리.
    private InputController _inputController = null;

    protected void SetInputController(InputController inputController)
        => _inputController = inputController;
    
    public InputController GetInputController()
        => _inputController;

    // 모델링 트랜스폼 관리.
    private Transform _visualTransform;

    protected void SetVisualTransform(Transform visualTransform)
        => _visualTransform = visualTransform;

    public Transform GetVisualTransform() 
        => _visualTransform;

    // 애니메이션 관리.
    private Animator _animator;

    protected void SetAnimator(Animator animator)
        => _animator = animator;

    public Animator GetAnimator()
        => _animator;

    // 물리 관리.
    private Rigidbody _rigid;

    protected void SetRigidBody(Rigidbody rigid)
        => _rigid = rigid;

    public Rigidbody GetRigidBody()
        => _rigid;
    
    // 리플레이 녹화용 변수.
    protected int _x, _y, _z;
    protected int _qx, _qy, _qz, _qw;
    protected int _vx, _vy, _vz;

    protected void ClearRecordVariables()
    {
        const int INVALID_VALUE = -10000;
        
        _x = INVALID_VALUE;
        _y = INVALID_VALUE;
        _z = INVALID_VALUE;

        _qx = INVALID_VALUE;
        _qy = INVALID_VALUE;
        _qz = INVALID_VALUE;
        _qw = INVALID_VALUE;

        _vx = INVALID_VALUE;
        _vy = INVALID_VALUE;
        _vz = INVALID_VALUE;
    }
    
    protected virtual void Awake()
    {
    }
    
    protected virtual void Start()
    {
    }
    
    protected virtual void OnDestroy()
    {
        if (UpdateManager.Instance != null)
            UpdateManager.Instance.MarkForRemoval(this);
    }

    public virtual void Init()
    {
        id = UniqueIdGenerator.Get();
        
        if (UpdateManager.Instance != null)
            UpdateManager.Instance.Add(this);
    }
    
    protected virtual void InitStat()
    {
    }
    
    protected virtual void InitVisual()
    {   
    }

    protected virtual void InitInput()
    {
    }
    
    protected virtual void InitAnimator()
    {
    }

    protected virtual void InitPhysics()
    {
    }

    protected virtual void Update()
    {
    }
    
    public virtual void OnFixedUpdate(float deltaTime)
    {
    }

    public virtual void OnFixedAfterUpdate(float deltaTime)
    {
    }
}