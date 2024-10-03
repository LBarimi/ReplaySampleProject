using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum BUTTON_TYPE
{
    NONE,
    RECORD,
    REPLAY,
    RECORD_STOP,
}

public sealed class UIButton : MonoBehaviour
{
    private static List<UIButton> _buttonList = new();
    
    public BUTTON_TYPE buttonType;

    private TextMeshProUGUI _text;
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        if (_buttonList.Contains(this) == false)
            _buttonList.Add(this);
    }

    private void OnDestroy()
    {
        if (_buttonList.Contains(this))
            _buttonList.Remove(this);
    }

    public void OnClick()
    {
        switch (buttonType)
        {
            case BUTTON_TYPE.RECORD:
            {
                buttonType = BUTTON_TYPE.RECORD_STOP;
                _text.text = "STOP";
                
                _buttonList.Find(x => x.buttonType == BUTTON_TYPE.REPLAY)._button.interactable = false;
                ReplayManager.StartRecord();
                break;
            }
            case BUTTON_TYPE.RECORD_STOP:
            {
                buttonType = BUTTON_TYPE.RECORD;
                _text.text = "REC";
                
                _buttonList.Find(x => x.buttonType == BUTTON_TYPE.REPLAY)._button.interactable = true;
                ReplayManager.StopRecord();
                break;
            }
            case BUTTON_TYPE.REPLAY:
            {
                _button.interactable = false;
                _buttonList.Find(x => x.buttonType == BUTTON_TYPE.RECORD)._button.interactable = true;
                ReplayManager.StartReplay();
                break;
            }
        }
    }
}