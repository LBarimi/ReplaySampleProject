using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public sealed class PlayerUnit : UnitBase
{
    public override void Init()
    {
        base.Init();
        
        InitStat();
        InitVisual();
        InitInput();
        InitAnimator();
        InitPhysics();
    }

    protected override void InitStat()
    {
        base.InitStat();
        
        SetStatController(new StatController());
        GetStatController().Init(GetId());
        GetStatController().GetStat(STAT_TYPE.MAX_HP).SetBaseValue(100);
        GetStatController().GetStat(STAT_TYPE.CUR_HP).SetBaseValue(100);
        GetStatController().GetStat(STAT_TYPE.MOVESPEED).SetBaseValue(5);
        GetStatController().GetStat(STAT_TYPE.ROTATIONSPEED).SetBaseValue(20);
        GetStatController().GetStat(STAT_TYPE.JUMPPOWER).SetBaseValue(10);
    }

    protected override void InitVisual()
    {
        base.InitVisual();

        var visualGo = Utils.SpawnGameObject("Visual_UnityChan", transform);

        if (visualGo == null)
            return;
        
        SetVisualTransform(visualGo.transform);
        
        // UnityChan VisualOffset 0, -1.0, 0
        GetVisualTransform().localPosition = new Vector3(0, -1.0f, 0);
        GetVisualTransform().localEulerAngles = Vector3.zero;
        GetVisualTransform().localScale = Vector3.one;
    }

    protected override void InitInput()
    {
        base.InitInput();
        
        SetInputController(new InputController());
        GetInputController().Init();
        GetInputController().Bind(INPUT_TYPE.LEFT, KeyCode.A);
        GetInputController().Bind(INPUT_TYPE.LEFT, KeyCode.LeftArrow);
        GetInputController().Bind(INPUT_TYPE.RIGHT, KeyCode.D);
        GetInputController().Bind(INPUT_TYPE.RIGHT, KeyCode.RightArrow);
        GetInputController().Bind(INPUT_TYPE.UP, KeyCode.W);
        GetInputController().Bind(INPUT_TYPE.UP, KeyCode.UpArrow);
        GetInputController().Bind(INPUT_TYPE.DOWN, KeyCode.S);
        GetInputController().Bind(INPUT_TYPE.DOWN, KeyCode.DownArrow);
        GetInputController().Bind(INPUT_TYPE.F1, KeyCode.F1);
        GetInputController().Bind(INPUT_TYPE.F2, KeyCode.F2);
        GetInputController().Bind(INPUT_TYPE.F3, KeyCode.F3);
    }

    protected override void InitAnimator()
    {
        base.InitAnimator();

        if (GetVisualTransform() == null)
            return;
        
        if (GetVisualTransform().TryGetComponent(typeof(Animator), out var com))
            SetAnimator(com as Animator);
        
        if (GetAnimator() == null)
            return;

        // Updates the animator during the physic loop in order to have the animation system synchronized with the physics engine.
        GetAnimator().updateMode = AnimatorUpdateMode.AnimatePhysics;
    }

    protected override void InitPhysics()
    {
        base.InitPhysics();

        if (TryGetComponent(typeof(Rigidbody), out var com))
            SetRigidBody(com as Rigidbody);
        
        if (GetRigidBody() == null)
            return;
        
        GetRigidBody().Sleep();
    }

    // 이 함수에서 발생하는 이벤트는 리플레이 데이터에 기록되지 않는다.
    protected override void Update()
    {
        var deltaTime = ReplayManager.GetDeltaTime();

        GetInputController().OnUpdate(deltaTime);
    }
    
    public override void OnFixedUpdate(float deltaTime)
    {
        // Movement.
        var input = GetInputController();
        var rigid = GetRigidBody();

        var inputVector = Vector2.zero;

        if (input.GetKey(INPUT_TYPE.LEFT))
            inputVector.x = -1.0f;
        else if (input.GetKey(INPUT_TYPE.RIGHT))
            inputVector.x = 1.0f;
        else
            inputVector.x = 0;

        if (input.GetKey(INPUT_TYPE.UP))
            inputVector.y = 1.0f;
        else if (input.GetKey(INPUT_TYPE.DOWN))
            inputVector.y = -1.0f;
        else
            inputVector.y = 0;

        // 걷는 애니메이션 재생.
        GetAnimator().SetFloat("Speed", inputVector == Vector2.zero ? 0 : 1);
        
        if (inputVector == Vector2.zero)
            return;
        
        // 이동.
        var pos = transform.position;
        pos.x += inputVector.x * GetStatController().GetStat(STAT_TYPE.MOVESPEED).GetFinal() * deltaTime;
        pos.z += inputVector.y * GetStatController().GetStat(STAT_TYPE.MOVESPEED).GetFinal() * deltaTime;
        transform.position = pos;
        
        // 회전.
        var lookDir = new Vector3(inputVector.x, 0, inputVector.y);
        var targetRotation = Quaternion.LookRotation(lookDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, GetStatController().GetStat(STAT_TYPE.ROTATIONSPEED).GetFinal() * deltaTime);
    }

    // 이 함수는 FixedUpdate랑 같은 프레임에 호출된다.
    // rigidbody.velocity를 설정하면 물리 연산이 엔진 내부에서 처리되기 때문에,
    // 여기서 위치와 회전 값을 다시 조정한다.
    // 조정은 소수점 둘째 자리까지만 남기고, 그 이하 숫자는 버린다.
    public override void OnFixedAfterUpdate(float deltaTime)
    {
        transform.SetTruncatePosition();
        transform.SetTruncateRotation();
    }
}