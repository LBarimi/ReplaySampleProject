using UnityEngine;

public sealed class InputData
{
    private KeyCode _keyCode;
    private bool _isDown;
    private bool _isPressed;
    private bool _isUp;
    private float _pressedTime;

    public InputData(KeyCode keyCode)
    {
        _keyCode = keyCode;

        _isDown = false;
        _isPressed = false;
        _isUp = false;

        _pressedTime = 0;
    }

    public KeyCode GetKeyCode()
        => _keyCode;

    public void SetDown(bool isDown)
        => _isDown = isDown;

    public void SetPressed(bool isPressed)
    {
        if (isPressed == false)
            _pressedTime = 0;

        _isPressed = isPressed;
    }
    public void SetUp(bool isUp)
        => _isUp = isUp;

    public bool IsDown()
        => _isDown;
    public bool IsPressed()
        => _isPressed;
    public bool IsUp()
        => _isUp;

    public float GetPressedTime()
        => _pressedTime;

    public void OnUpdate(float deltaTime)
    {
        if (_isPressed == false)
            return;
        
        _pressedTime += deltaTime;
    }
}