using UnityEngine;

public class StatData
{
    private int _baseValue;
    
    private int _add;
    private int _percentAdd;
    private int _multiply;

    private float _getFinal;
    private bool _isDirty;

    public StatData()
    {
        _baseValue = 0;
        _add = 0;
        _percentAdd = 0;
        _multiply = 100;
        
        SetDirty();
    }
    
    public StatData(int value)
    {
        _baseValue = value;
        _add = 0;
        _percentAdd = 0;
        _multiply = 100;
        
        SetDirty();
    }

    public void Clear()
    {
        _add = 0;
        _percentAdd = 0;
        _multiply = 100;
        
        SetDirty();
    }
    
    public void Clear(int value)
    {
        _baseValue = value;
        _add = 0;
        _percentAdd = 0;
        _multiply = 100;
        
        SetDirty();
    }
    
    public void SetBaseValue(int value)
    {
        _baseValue = value;
        SetDirty();
    }

    public int GetBaseValue() 
        => _baseValue;
    
    public void Add(int value)
    {
        _add += value;
        SetDirty();
    }
    
    public void AddPercent(int value)
    {
        _percentAdd += value;
        SetDirty();
    }

    public int GetPercent() 
        => _percentAdd;

    public void AddMultiply(int value)
    {
        _multiply += value;
        SetDirty();
    }

    public int GetMultiply() 
        => _multiply;

    public float GetFinal()
    {
        if (_isDirty == false)
            return _getFinal;

        var percentAdd = 1 + _percentAdd.ToRate();
        percentAdd = Mathf.Max(0, percentAdd);
        
        _getFinal = (_baseValue + _add.ToRate()) * (percentAdd) * _multiply.ToRate();
        ReleaseDirty();

        return _getFinal;
    }

    public int GetFinalInt()
    {
        if (_isDirty == false)
            return Mathf.RoundToInt(_getFinal);

        var percentAdd = 1 + _percentAdd.ToRate();
        percentAdd = Mathf.Max(0, percentAdd);
        
        _getFinal = (_baseValue + _add.ToRate()) * (percentAdd) * _multiply.ToRate();
        ReleaseDirty();

        return Mathf.RoundToInt(_getFinal);
    }
    
    private void SetDirty() 
        => _isDirty = true;

    private void ReleaseDirty()
        => _isDirty = false;
}