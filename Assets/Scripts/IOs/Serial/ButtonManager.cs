using System;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    int ButtonType;
    private int _insideColliderCount = 0;

    public static event Action buttonDidChange;
    void Start()
    {
        ButtonType = (int)Enum.Parse(typeof(Button), gameObject.name);
    }
    private void OnTriggerEnter(Collider other)
    {
        _insideColliderCount += 1;
        SerialManager.ChangeTouch(true, (int)ButtonType, true);
        buttonDidChange?.Invoke();
    }
    private void OnTriggerExit(Collider other)
    {
        _insideColliderCount -= 1;
        _insideColliderCount = Mathf.Max(0, _insideColliderCount);
        if (_insideColliderCount == 0)
        {
            SerialManager.ChangeTouch(true, (int)ButtonType, false);
            buttonDidChange?.Invoke();
        }
    }
    enum Button
    {
        MAI2_IO_GAMEBTN_1 = 0x01,
        MAI2_IO_GAMEBTN_2 = 0x02,
        MAI2_IO_GAMEBTN_3 = 0x04,
        MAI2_IO_GAMEBTN_4 = 0x08,
        MAI2_IO_GAMEBTN_5 = 0x10,
        MAI2_IO_GAMEBTN_6 = 0x20,
        MAI2_IO_GAMEBTN_7 = 0x40,
        MAI2_IO_GAMEBTN_8 = 0x80,
        MAI2_IO_GAMEBTN_SELECT = 0x100,
    };

}
