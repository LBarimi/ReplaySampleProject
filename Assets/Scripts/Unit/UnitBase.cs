using UnityEngine;

public class UnitBase : MonoBehaviour, IUpdater, IObject
{
    public bool IsMarkedForRemoval { get; set; }

    // 오브젝트 식별자.
    public ulong id { get; set; }

    public ulong GetId() 
        => id;

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