using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class UnitManager : MonoBehaviour
{
    private static Dictionary<int, UnitBase> _unitDict = new();

    public static List<UnitBase> GetAllUnitList() 
        => _unitDict.Values.ToList();

    public static bool Has(int id)
    {
        return _unitDict.ContainsKey(id);
    }

    public static void Add(int id, UnitBase unit)
    {
        _unitDict.TryAdd(id, unit);
    }

    public static bool Remove(int id)
    {
        return _unitDict.Remove(id);
    }

    public static UnitBase Get(int id)
    {
        return _unitDict.GetValueOrDefault(id);
    }

    public static void Clear()
    {
        _unitDict.Clear();
    }
}