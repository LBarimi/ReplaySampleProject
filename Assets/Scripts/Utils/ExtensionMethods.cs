using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static float ConvertBytesToKB(this int bytes)
    {
        return bytes / 1024f;
    }
    
    public static float ToRate(this int value)
    {
        return value * 0.01f;
    }
    
    public static float ToRate(this float value)
    {
        return value * 0.01f;
    }
    
    public static int ToPercentage(this float value)
    {
        return (int)(value * 100);
    }
    
    public static int ToPercentage(this int value)
    {
        return value * 100;
    }
    
    public static float Truncate(this float value)
    {
        return (int)(value * 100f) / 100f;
    }

    public static Vector2 Truncate(this Vector2 value)
    {
        var d = value;
        d.x = (int)(d.x * 100f) / 100f;
        d.y = (int)(d.y * 100f) / 100f;
        return d;
    }
    
    public static Vector3 Truncate(this Vector3 value)
    {
        var d = value;
        d.x = (int)(d.x * 100f) / 100f;
        d.y = (int)(d.y * 100f) / 100f;
        d.z = (int)(d.z * 100f) / 100f;
        return d;
    }
    
    public static Quaternion Truncate(this Quaternion value)
    {
        var d = value;
        d.x = (int)(d.x * 100f) / 100f;
        d.y = (int)(d.y * 100f) / 100f;
        d.z = (int)(d.z * 100f) / 100f;
        d.w = (int)(d.w * 100f) / 100f;
        return d;
    }

    public static void SetTruncatePosition(this Transform transform)
    {
        transform.position = transform.position.Truncate();
    }
    
    public static void SetTruncateRotation(this Transform transform)
    {
        transform.rotation = transform.rotation.Truncate();
    }

    public static bool HasEnum<T>(this string value)
    {
        return Enum.IsDefined(typeof(T), value);
    }
    
    public static bool IsNullOrEmpty(this IList list)
    {
        return list == null || list.Count == 0;
    }

    public static bool IsNullOrEmpty(this IReadOnlyList<ulong> list)
    {
        return list == null || list.Count == 0;
    }
    
    public static bool IsNaN(this Vector2 vector)
    {
        return float.IsNaN(vector.x) && float.IsNaN(vector.y);
    }
    
    public static bool IsNaN(this Vector3 vector)
    {
        return float.IsNaN(vector.x) && float.IsNaN(vector.y) && float.IsNaN(vector.z);
    }

    public static T IndexOrNull<T>(this IList<T> list, int index) where T : class
    {
        if (list == null || index < 0 || list.Count <= index)
        {
            return null;
        }

        return list[index];
    }

    public static T IndexOrDefault<T>(this IList<T> list, int index, T defaultValue = default) where T : struct
    {
        if (list == null || index < 0 || list.Count <= index)
        {
            return defaultValue;
        }

        return list[index];
    }

    public static T ToEnum<T>(this string value)
    {
        if (!Enum.IsDefined(typeof(T), value))
            return default(T);

        return (T)Enum.Parse(typeof(T), value, true);
    }
}