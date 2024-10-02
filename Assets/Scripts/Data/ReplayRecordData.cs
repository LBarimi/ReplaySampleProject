using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public sealed class ReplayRecordData
{
    [SerializeField]
    private ulong f; // curFixedStep.

    [SerializeField]
    private REPLAY_ACTION_TYPE t; // type.

    [SerializeField]
    private string s; // str.
    
    [SerializeField]
    private int _x, _y, _z;
    [SerializeField]
    private int _qx, _qy, _qz, _qw;
    [SerializeField]
    private int _vx, _vy, _vz;

    [SerializeField]
    private bool i; // input.

    public void SetType(REPLAY_ACTION_TYPE type)
        => t = type;

    public REPLAY_ACTION_TYPE GetType()
        => t;
    
    public void SetCurFixedStep(ulong curFixedStep)
        => f = curFixedStep;

    public ulong GetCurFixedStep()
        => f;

    public void SetKey(string key)
        => s = key;

    public string GetKey()
        => s;
    
    public void SetPosition(int x, int y, int z)
    {
        _x = x;
        _y = y;
        _z = z;
    }

    public (int x, int y, int z) GetPosition()
        => (_x, _y, _z);

    public void SetQuaternion(int qx, int qy, int qz, int qw)
    {
        _qx = qx;
        _qy = qy;
        _qz = qz;
        _qw = qw;
    }

    public (int qx, int qy, int qz, int qw) GetQuaternion()
        => (_qx, _qy, _qz, _qw);
    
    public void SetVelocity(int vx, int vy, int vz)
    {
        _vx = vx;
        _vy = vy;
        _vz = vz;
    }

    public (int vx, int vy, int vz) GetVelocity()
        => (_vx, _vy, _vz);

    public void SetInput(bool isInput)
    {
        i = isInput;
    }

    public bool GetInput()
        => i;
    
    private int GetStringSize(string s)
    {
        return string.IsNullOrEmpty(s) ? 0 : System.Text.Encoding.UTF8.GetByteCount(s);
    }
    
    public int GetSizeInBytes()
    {
        var size = 0;

        size += sizeof(ulong);
        size += sizeof(int) * 12;
        size += sizeof(bool);
        
        if (string.IsNullOrEmpty(s) == false)
        {
            size += sizeof(char) * s.Length;
        }

        size += IntPtr.Size;
        
        return size;
    }
    
    public int GetJsonSizeInBytes()
    {
        var size = 0;

        size += GetStringSize("\"f\":") + f.ToString().Length;
        size += GetStringSize("\"t\":") + GetStringSize(t.ToString());
        size += GetStringSize("\"s\":") + GetStringSize(s);
        size += GetStringSize("\"_x\":") + _x.ToString().Length;
        size += GetStringSize("\"_y\":") + _y.ToString().Length;
        size += GetStringSize("\"_z\":") + _z.ToString().Length;
        size += GetStringSize("\"_qx\":") + _qx.ToString().Length;
        size += GetStringSize("\"_qy\":") + _qy.ToString().Length;
        size += GetStringSize("\"_qz\":") + _qz.ToString().Length;
        size += GetStringSize("\"_qw\":") + _qw.ToString().Length;
        size += GetStringSize("\"_vx\":") + _vx.ToString().Length;
        size += GetStringSize("\"_vy\":") + _vy.ToString().Length;
        size += GetStringSize("\"_vz\":") + _vz.ToString().Length;
        size += GetStringSize("\"i\":") + (i ? "true".Length : "false".Length);

        size += 2;
        size += 11;
    
        return size;
    }
}