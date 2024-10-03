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
    JUMP,
    F1,
    F2,
    F3,
    MAX,
}

public sealed class InputController
{
    private Dictionary<INPUT_TYPE, List<InputData>> InputDict { get; set; } = new();

    public Vector2Int movementInputValue;
    
    public void Init()
    {
        Clear();
    }
    
    public void Clear()
    {
        InputDict.Clear();
        movementInputValue = Vector2Int.zero;
    }

    public void ReleaseDownUp()
    {
        for (var inputType = INPUT_TYPE.NONE; inputType < INPUT_TYPE.MAX; inputType++)
        {
            if (InputDict.TryGetValue(inputType, out var value) == false)
                continue;
            
            foreach (var item in value)
            {
                item.SetDown(false);
                item.SetUp(false);
            }
        }
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
    
    public void SetKey(INPUT_TYPE inputType, bool isPressed)
    {
        if (InputDict.TryGetValue(inputType, out var value))
        {
            foreach (var item in value)
            {
                item.SetPressed(isPressed);
            }
        }
    }

    public void SetKeyDown(INPUT_TYPE inputType, bool isDown)
    {
        if (InputDict.TryGetValue(inputType, out var value))
        {
            foreach (var item in value)
            {
                item.SetDown(isDown);
            }
        }
    }

    public void SetKeyUp(INPUT_TYPE inputType, bool isUp)
    {
        if (InputDict.TryGetValue(inputType, out var value))
        {
            foreach (var item in value)
            {
                item.SetUp(isUp);
            }
        }
    }
    
    public void OnUpdate(float deltaTime)
    {
        for (var inputType = INPUT_TYPE.NONE; inputType < INPUT_TYPE.MAX; inputType++)
        {
            OnUpdate(inputType, deltaTime);
        }
    }

    public void OnDownUpdate(float deltaTime)
    {
        for (var inputType = INPUT_TYPE.NONE; inputType < INPUT_TYPE.MAX; inputType++)
        {
            OnDownUpdate(inputType, deltaTime);
        }
    }
    
    private void OnUpdate(INPUT_TYPE inputType, float deltaTime)
    {
        if (InputDict.TryGetValue(inputType, out var value) == false)
            return;
        
        foreach (var item in value)
        {
            if (item.IsDown() == false)
                item.SetDown(Input.GetKeyDown(item.GetKeyCode()));
            item.SetPressed(Input.GetKey(item.GetKeyCode()));
            item.OnUpdate(deltaTime);
        }
    }

    private void OnDownUpdate(INPUT_TYPE inputType, float deltaTime)
    {
        if (InputDict.TryGetValue(inputType, out var value) == false)
            return;
        
        foreach (var item in value)
        {
            item.SetDown(Input.GetKeyDown(item.GetKeyCode()));
            item.SetUp(Input.GetKeyUp(item.GetKeyCode()));
        }
    }
}