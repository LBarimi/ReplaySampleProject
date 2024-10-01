using System;
using UnityEngine;

public sealed class ReplayRecordData
{
    private ulong _curFixedStep;

    private REPLAY_ACTION_TYPE _type;

    private string _key;
    
    private int _x, _y, _z;
    private int _qx, _qy, _qz, _qw;
    private int _vx, _vy, _vz;

    private bool _isInput;

    public void SetType(REPLAY_ACTION_TYPE type)
        => _type = type;
    
    public void SetCurFixedStep(ulong curFixedStep)
        => _curFixedStep = curFixedStep;

    public void SetKey(string key)
        => _key = key;
    
    public void SetPosition(int x, int y, int z)
    {
        _x = x;
        _y = y;
        _z = z;
    }

    public void SetQuaternion(int qx, int qy, int qz, int qw)
    {
        _qx = qx;
        _qy = qy;
        _qz = qz;
        _qw = qw;
    }

    public void SetVelocity(int vx, int vy, int vz)
    {
        _vx = vx;
        _vy = vy;
        _vz = vz;
    }

    public void SetInput(bool isInput)
    {
        _isInput = isInput;
    }
    
    public int GetSizeInBytes()
    {
        var size = 0;

        size += sizeof(ulong);
        size += sizeof(int) * 12;
        size += sizeof(bool);
        
        if (string.IsNullOrEmpty(_key) == false)
        {
            size += sizeof(char) * _key.Length;
        }

        size += IntPtr.Size;
        
        return size;
    }
}