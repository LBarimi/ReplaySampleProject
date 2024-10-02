using System;
using UnityEngine;

public class UnitBase : MonoBehaviour, IUpdater, IObject
{
    public bool IsMarkedForRemoval { get; set; }

    // 오브젝트 식별자.
    public ulong id { get; set; }

    public ulong GetId() 
        => id;

    public string _visualKey { get; set; }

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
    
    // 업데이트 함수에 연결된 콜백 목록.
    protected Action<float> _onFixedUpdate_Before;
    protected Action<float> _onFixedUpdate_After;
    
    protected Action<float> _onFixedAfterUpdate_Before;
    protected Action<float> _onFixedAfterUpdate_After;
    
    public void SetFixedUpdateBefore(Action<float> callback) 
        => _onFixedUpdate_Before += callback;

    public void SetFixedUpdateAfter(Action<float> callback) 
        => _onFixedUpdate_After += callback;

    public void SetFixedAfterUpdateBefore(Action<float> callback) 
        => _onFixedAfterUpdate_Before += callback;

    public void SetFixedAfterUpdateAfter(Action<float> callback) 
        => _onFixedAfterUpdate_After += callback;

    public void RemoveFixedUpdateBefore(Action<float> callback) 
        => _onFixedUpdate_Before -= callback;

    public void RemoveFixedUpdateAfter(Action<float> callback) 
        => _onFixedUpdate_After -= callback;

    public void RemoveFixedAfterUpdateBefore(Action<float> callback)
        => _onFixedAfterUpdate_Before -= callback;

    public void RemoveFixedAfterUpdateAfter(Action<float> callback) 
        => _onFixedAfterUpdate_After -= callback;

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