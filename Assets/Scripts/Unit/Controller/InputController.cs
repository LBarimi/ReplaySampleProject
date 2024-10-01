using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum INPUT_TYPE
{
    NONE,
    LEFT,
    RIGHT,
    UP,
    DOWN,
    F1,
    F2,
    F3,
    MAX,
}

public sealed class InputController
{
    private Dictionary<INPUT_TYPE, List<InputData>> InputDict { get; set; } = new();

    public void Init()
    {
        Clear();
    }
    
    public void Clear()
    {
        InputDict.Clear();
    }

    public void Bind(INPUT_TYPE inputType, KeyCode keyCode)
    {
        if (InputDict.ContainsKey(inputType) == false)
        {
            InputDict.Add(inputType, new List<InputData>());
        }

        if (InputDict[inputType].Any(item => item.GetKeyCode() == keyCode))
        {
            return;
        }
        
        InputDict[inputType].Add(new InputData(keyCode));
    }

    // Pressed.
    public bool GetKey(INPUT_TYPE inputType)
    {
        if (InputDict.TryGetValue(inputType, out var value) == false)
            return false;

        return value.Any(item => item.IsPressed());
    }
    
    public bool GetKeyDown(INPUT_TYPE inputType)
    {
        if (InputDict.TryGetValue(inputType, out var value) == false)
            return false;

        return value.Any(item => item.IsDown());
    }
    
    public bool GetKeyUp(INPUT_TYPE inputType)
    {
        if (InputDict.TryGetValue(inputType, out var value) == false)
            return false;

        return value.Any(item => item.IsUp());
    }
    
    public void OnUpdate(float deltaTime)
    {
        for (var inputType = INPUT_TYPE.NONE; inputType < INPUT_TYPE.MAX; inputType++)
        {
            OnUpdate(inputType, deltaTime);
        }
    }

    private void OnUpdate(INPUT_TYPE inputType, float deltaTime)
    {
        if (InputDict.TryGetValue(inputType, out var value) == false)
            return;
        
        foreach (var item in value)
        {
            item.SetDown(Input.GetKeyDown(item.GetKeyCode()));
            item.SetPressed(Input.GetKey(item.GetKeyCode()));
            item.SetUp(Input.GetKeyUp(item.GetKeyCode()));
            item.OnUpdate(deltaTime);
        }
    }
}