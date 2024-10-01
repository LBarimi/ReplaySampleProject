using System.ComponentModel;
using UnityEngine;

public static class Utils
{

    public static GameObject SpawnGameObject(string key, Transform parent = null)
    {
        var path = DataManager.GetString(key);
        var res = Resources.Load(path, typeof(GameObject)) as GameObject;
        var go = Object.Instantiate(res, parent, true);
        go.name = key;
        go.transform.position = Vector3.zero;
        go.transform.rotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
        return go;
    }
}