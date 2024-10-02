using UnityEngine;

// 트랜스폼의 변동 사항을 검사하고 레코더에 데이터를 전달하는 역할의 클래스.
public sealed class ReplayTransformRecorder : MonoBehaviour
{
    private Rigidbody _rigid;
    
    // 리플레이 녹화용 변수.
    private int _x, _y, _z;
    private int _qx, _qy, _qz, _qw;
    private int _vx, _vy, _vz;

    private void ClearRecordVariables()
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

    private void Awake()
    {
        UnitBase target = null;

        if (TryGetComponent(typeof(UnitBase), out var com))
            target = com as UnitBase;

        if (target == null)
            return;
        
        target.SetFixedUpdateAfter(OnFixedUpdate);
        target.SetFixedAfterUpdateAfter(OnFixedAfterUpdate);
        
        if (TryGetComponent(typeof(Rigidbody), out com))
            _rigid = com as Rigidbody;
        
        ClearRecordVariables();
    }
    
    private void OnFixedUpdate(float deltaTime)
    {
        if (_rigid == null)
            return;
        
        // 녹화용 데이터 설정.
        var velocity = _rigid.velocity;

        var vx = velocity.x.ToPercentage();
        var vy = velocity.y.ToPercentage();
        var vz = velocity.z.ToPercentage();
        
        if (vx != _vx || vy != _vy || vz != _vz)
        {
            _vx = vx;
            _vy = vy;
            _vz = vz;
            
            ReplayRecorder.Set(REPLAY_ACTION_TYPE.SYNC_VELOCITY, velocity.x, velocity.y, velocity.z);
        }
    }

    private void OnFixedAfterUpdate(float deltaTime)
    {
        // 녹화용 데이터 설정.
        var pos = transform.position;
        var quaternion = transform.rotation;

        var x = pos.x.ToPercentage();
        var y = pos.y.ToPercentage();
        var z = pos.z.ToPercentage();
        
        var qx = quaternion.x.ToPercentage();
        var qy = quaternion.y.ToPercentage();
        var qz = quaternion.z.ToPercentage();
        var qw = quaternion.w.ToPercentage();

        // Transform의 정보가 변경되었는가
        if (x != _x || y != _y || z != _z || qx != _qx || qy != _qy || qz != _qz || qw != _qw)
        {
            _x = x;
            _y = y;
            _z = z;
            
            _qx = qx;
            _qy = qy;
            _qz = qz;
            _qw = qw;

            ReplayRecorder.Set(REPLAY_ACTION_TYPE.SYNC_TRANSFORM, pos.x, pos.y, pos.z, quaternion.x, quaternion.y, quaternion.z, quaternion.w);
        }
    }
}