using System.Collections.Generic;
using UnityEngine;

public enum STAT_TYPE
{
    NONE,
    CUR_HP,
    MAX_HP,
    MOVESPEED,
    ROTATIONSPEED,
    JUMPPOWER,
}

public sealed class StatController
{
    private Dictionary<STAT_TYPE, StatData> StatDict { get; set; } = new();

    public void Init()
    {
        Clear();
    }
    
    public void Clear()
    {
        StatDict.Clear();
    }
    
    public StatData GetStat(STAT_TYPE statType)
    {
        if (StatDict.ContainsKey(statType) == false)
        {
            var d = new StatData();
            StatDict.Add(statType, d);
        }

        return StatDict[statType];
    }
}